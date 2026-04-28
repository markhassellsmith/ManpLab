#include "BatchRenderer.h"
#include "FractalEngineWrapper.h"
#include <algorithm>
#include <cmath>

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Diagnostics;
using namespace System::IO;
using namespace ManpCore::Native;

//=============================================================================
// BatchRenderer Implementation
//=============================================================================

BatchRenderer::BatchRenderer()
{
    m_jobs = gcnew List<BatchJob^>();
    m_engine = gcnew FractalEngineWrapper();
    m_isProcessing = false;
    m_cancellationRequested = false;
}

BatchRenderer::~BatchRenderer()
{
    this->!BatchRenderer();
}

BatchRenderer::!BatchRenderer()
{
    if (m_engine != nullptr)
    {
        delete m_engine;
        m_engine = nullptr;
    }
}

//=============================================================================
// Job Management
//=============================================================================

BatchJob^ BatchRenderer::AddJob(String^ name, FractalParameters^ parameters, String^ outputPath)
{
    if (parameters == nullptr)
        throw gcnew ArgumentNullException("parameters");

    auto job = gcnew BatchJob();
    job->Name = (name != nullptr) ? name : String::Format("Job {0}", m_jobs->Count + 1);
    job->Parameters = parameters;
    job->OutputPath = outputPath;
    job->Status = BatchJobStatus::Pending;
    job->Progress = 0.0;

    m_jobs->Add(job);

    return job;
}

int BatchRenderer::CreateAnimation(String^ name, List<AnimationKeyframe^>^ keyframes, int fps, InterpolationMode mode, String^ outputDirectory)
{
    if (keyframes == nullptr || keyframes->Count < 2)
        throw gcnew ArgumentException("Animation requires at least 2 keyframes");

    if (fps <= 0)
        throw gcnew ArgumentException("FPS must be positive");

    // Sort keyframes by time - C++/CLI doesn't support lambdas in managed code
    // Use a simple bubble sort instead
    auto sortedKeyframes = gcnew List<AnimationKeyframe^>(keyframes);
    for (int i = 0; i < sortedKeyframes->Count - 1; i++)
    {
        for (int j = 0; j < sortedKeyframes->Count - i - 1; j++)
        {
            if (sortedKeyframes[j]->Time > sortedKeyframes[j + 1]->Time)
            {
                auto temp = sortedKeyframes[j];
                sortedKeyframes[j] = sortedKeyframes[j + 1];
                sortedKeyframes[j + 1] = temp;
            }
        }
    }

    // Calculate total duration
    double totalDuration = 0.0;
    for (int i = 0; i < sortedKeyframes->Count - 1; i++)
    {
        totalDuration += sortedKeyframes[i]->Duration;
    }

    // Calculate total number of frames
    int totalFrames = (int)(totalDuration * fps);
    if (totalFrames <= 0)
        totalFrames = fps; // At least 1 second

    // Generate frames
    int frameNumber = 0;
    double currentTime = 0.0;
    double timeStep = totalDuration / totalFrames;

    for (int frame = 0; frame < totalFrames; frame++)
    {
        // Find which keyframe segment we're in
        double segmentStart = 0.0;
        int keyframeIndex = 0;

        for (int i = 0; i < sortedKeyframes->Count - 1; i++)
        {
            double segmentEnd = segmentStart + sortedKeyframes[i]->Duration;
            if (currentTime <= segmentEnd)
            {
                keyframeIndex = i;
                break;
            }
            segmentStart = segmentEnd;
        }

        // Interpolate parameters between keyframes
        auto startKeyframe = sortedKeyframes[keyframeIndex];
        auto endKeyframe = sortedKeyframes[Math::Min(keyframeIndex + 1, sortedKeyframes->Count - 1)];

        double segmentProgress = (currentTime - segmentStart) / startKeyframe->Duration;
        segmentProgress = Math::Max(0.0, Math::Min(1.0, segmentProgress)); // Clamp to [0,1]

        auto frameParams = InterpolateParameters(
            startKeyframe->Parameters,
            endKeyframe->Parameters,
            segmentProgress,
            mode
        );

        // Create output path if directory specified
        String^ framePath = nullptr;
        if (!String::IsNullOrEmpty(outputDirectory))
        {
            framePath = Path::Combine(outputDirectory, 
                String::Format("{0}_frame_{1:D6}.png", name, frameNumber));
        }

        // Add job for this frame
        String^ frameName = String::Format("{0} - Frame {1}/{2}", name, frameNumber + 1, totalFrames);
        AddJob(frameName, frameParams, framePath);

        currentTime += timeStep;
        frameNumber++;
    }

    return frameNumber;
}

//=============================================================================
// Processing
//=============================================================================

void BatchRenderer::ProcessAll()
{
    if (m_isProcessing)
        throw gcnew InvalidOperationException("Batch is already processing");

    m_isProcessing = true;
    m_cancellationRequested = false;

    try
    {
        int totalJobs = m_jobs->Count;
        int currentJobIndex = 0;

        for each (auto job in m_jobs)
        {
            if (m_cancellationRequested)
            {
                // Mark remaining jobs as cancelled
                if (job->Status == BatchJobStatus::Pending)
                {
                    job->Status = BatchJobStatus::Cancelled;
                }
                continue;
            }

            if (job->Status != BatchJobStatus::Pending)
                continue; // Skip non-pending jobs

            currentJobIndex++;

            // Process this job
            ProcessJobInternal(job);

            // Fire progress event
            double overallProgress = (currentJobIndex * 100.0) / totalJobs;
            auto progressArgs = gcnew BatchProgressEventArgs(job, overallProgress, currentJobIndex, totalJobs);

            ProgressChanged(this, progressArgs);
            JobCompleted(this, progressArgs);
        }

        // Fire batch completed event
        BatchCompleted(this, EventArgs::Empty);
    }
    finally
    {
        m_isProcessing = false;
    }
}

void BatchRenderer::ProcessJobInternal(BatchJob^ job)
{
    job->Status = BatchJobStatus::Running;
    job->StartedTime = DateTime::Now;
    job->Progress = 0.0;

    try
    {
        // Render the fractal using Calculate method
        auto result = m_engine->Calculate(job->Parameters);

        job->ImageData = result->PixelData;
        job->Progress = 100.0;

        // Note: File saving will be handled by C# layer using ImageExportService
        // The BatchRenderer just produces the image data
        // OutputPath is stored for reference but not used in native code

        job->Status = BatchJobStatus::Completed;
        job->CompletedTime = DateTime::Now;
    }
    catch (Exception^ ex)
    {
        job->Status = BatchJobStatus::Failed;
        job->ErrorMessage = ex->Message;
        job->CompletedTime = DateTime::Now;

        // Don't throw - continue processing other jobs
        Debug::WriteLine(String::Format("Job '{0}' failed: {1}", job->Name, ex->Message));
    }
}

void BatchRenderer::CancelAll()
{
    m_cancellationRequested = true;
}

void BatchRenderer::ClearJobs(bool includePending)
{
    if (m_isProcessing && includePending)
        throw gcnew InvalidOperationException("Cannot clear pending jobs while processing");

    auto jobsToRemove = gcnew List<BatchJob^>();

    for each (auto job in m_jobs)
    {
        if (includePending || (job->Status != BatchJobStatus::Pending))
        {
            jobsToRemove->Add(job);
        }
    }

    for each (auto job in jobsToRemove)
    {
        m_jobs->Remove(job);
    }
}

//=============================================================================
// Interpolation
//=============================================================================

FractalParameters^ BatchRenderer::InterpolateParameters(
    FractalParameters^ start, 
    FractalParameters^ end, 
    double t, 
    InterpolationMode mode)
{
    if (start == nullptr || end == nullptr)
        throw gcnew ArgumentNullException("start and end parameters cannot be null");

    // Apply interpolation curve
    double curvedT = ApplyInterpolationCurve(t, mode);

    auto result = gcnew FractalParameters();

    // Copy fractal type and dimensions from start
    result->FractalType = start->FractalType;
    result->Width = start->Width;
    result->Height = start->Height;
    result->Palette = start->Palette;

    // Interpolate iterations (integer, so round)
    result->MaxIterations = (int)(start->MaxIterations + curvedT * (end->MaxIterations - start->MaxIterations));

    // Interpolate center coordinates
    result->CenterX = start->CenterX + curvedT * (end->CenterX - start->CenterX);
    result->CenterY = start->CenterY + curvedT * (end->CenterY - start->CenterY);

    // Interpolate view size (use exponential for zoom to maintain constant zoom speed)
    if (mode == InterpolationMode::Exponential)
    {
        // Logarithmic interpolation for exponential zoom
        double logStart = Math::Log(start->ViewWidth);
        double logEnd = Math::Log(end->ViewWidth);
        result->ViewWidth = Math::Exp(logStart + curvedT * (logEnd - logStart));

        logStart = Math::Log(start->ViewHeight);
        logEnd = Math::Log(end->ViewHeight);
        result->ViewHeight = Math::Exp(logStart + curvedT * (logEnd - logStart));
    }
    else
    {
        // Linear interpolation for view size
        result->ViewWidth = start->ViewWidth + curvedT * (end->ViewWidth - start->ViewWidth);
        result->ViewHeight = start->ViewHeight + curvedT * (end->ViewHeight - start->ViewHeight);
    }

    // Interpolate Julia parameters
    result->IsJuliaSet = start->IsJuliaSet;
    if (result->IsJuliaSet)
    {
        result->JuliaCX = start->JuliaCX + curvedT * (end->JuliaCX - start->JuliaCX);
        result->JuliaCY = start->JuliaCY + curvedT * (end->JuliaCY - start->JuliaCY);
    }

    // TODO: High-precision BigDouble interpolation for deep zoom animations
    // For now, high-precision animations not supported

    return result;
}

double BatchRenderer::ApplyInterpolationCurve(double t, InterpolationMode mode)
{
    // Clamp to [0, 1]
    t = Math::Max(0.0, Math::Min(1.0, t));

    switch (mode)
    {
        case InterpolationMode::Linear:
            return t;

        case InterpolationMode::EaseIn:
            // Quadratic ease in: t^2
            return t * t;

        case InterpolationMode::EaseOut:
            // Quadratic ease out: 1 - (1-t)^2
            return 1.0 - (1.0 - t) * (1.0 - t);

        case InterpolationMode::EaseInOut:
            // Cubic ease in-out
            if (t < 0.5)
                return 4.0 * t * t * t;
            else
            {
                double f = 2.0 * t - 2.0;
                return 0.5 * f * f * f + 1.0;
            }

        case InterpolationMode::Exponential:
            // Exponential handled separately in InterpolateParameters for zoom
            return t;

        default:
            return t;
    }
}

//=============================================================================
// Properties
//=============================================================================

int BatchRenderer::PendingJobCount::get()
{
    int count = 0;
    for each (auto job in m_jobs)
    {
        if (job->Status == BatchJobStatus::Pending)
            count++;
    }
    return count;
}

int BatchRenderer::CompletedJobCount::get()
{
    int count = 0;
    for each (auto job in m_jobs)
    {
        if (job->Status == BatchJobStatus::Completed)
            count++;
    }
    return count;
}

int BatchRenderer::FailedJobCount::get()
{
    int count = 0;
    for each (auto job in m_jobs)
    {
        if (job->Status == BatchJobStatus::Failed)
            count++;
    }
    return count;
}

double BatchRenderer::OverallProgress::get()
{
    if (m_jobs->Count == 0)
        return 0.0;

    int totalCompleted = CompletedJobCount + FailedJobCount;
    return (totalCompleted * 100.0) / m_jobs->Count;
}
