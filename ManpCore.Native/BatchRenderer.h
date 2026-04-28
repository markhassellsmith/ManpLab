#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace ManpCore {
namespace Native {

    // Forward declaration
    ref class FractalParameters;
    ref class FractalEngineWrapper;

    /// <summary>
    /// Status of a batch render job
    /// </summary>
    public enum class BatchJobStatus
    {
        /// <summary>Job is queued but not started</summary>
        Pending,
        /// <summary>Job is currently rendering</summary>
        Running,
        /// <summary>Job completed successfully</summary>
        Completed,
        /// <summary>Job failed with error</summary>
        Failed,
        /// <summary>Job was cancelled by user</summary>
        Cancelled
    };

    /// <summary>
    /// Interpolation method for animation frames
    /// </summary>
    public enum class InterpolationMode
    {
        /// <summary>Linear interpolation (constant speed)</summary>
        Linear,
        /// <summary>Ease in (slow start, fast end)</summary>
        EaseIn,
        /// <summary>Ease out (fast start, slow end)</summary>
        EaseOut,
        /// <summary>Ease in-out (slow start, fast middle, slow end)</summary>
        EaseInOut,
        /// <summary>Exponential zoom (logarithmic scale)</summary>
        Exponential
    };

    /// <summary>
    /// Represents a single render job in the batch queue
    /// </summary>
    public ref class BatchJob
    {
    public:
        /// <summary>Unique identifier for this job</summary>
        property Guid JobId;

        /// <summary>User-friendly name for this job</summary>
        property String^ Name;

        /// <summary>Fractal parameters for this render</summary>
        property FractalParameters^ Parameters;

        /// <summary>Output file path (optional, null for in-memory only)</summary>
        property String^ OutputPath;

        /// <summary>Current status of the job</summary>
        property BatchJobStatus Status;

        /// <summary>Progress percentage (0-100)</summary>
        property double Progress;

        /// <summary>Error message if Status is Failed</summary>
        property String^ ErrorMessage;

        /// <summary>Rendered image data (BGRA format)</summary>
        property array<unsigned char>^ ImageData;

        /// <summary>Time when job was created</summary>
        property DateTime CreatedTime;

        /// <summary>Time when job started rendering</summary>
        property DateTime StartedTime;

        /// <summary>Time when job completed</summary>
        property DateTime CompletedTime;

        BatchJob()
        {
            JobId = Guid::NewGuid();
            Status = BatchJobStatus::Pending;
            Progress = 0.0;
            CreatedTime = DateTime::Now;
        }
    };

    /// <summary>
    /// Event arguments for batch renderer progress updates
    /// </summary>
    public ref class BatchProgressEventArgs : EventArgs
    {
    public:
        /// <summary>Job that triggered this progress event</summary>
        property BatchJob^ Job;

        /// <summary>Overall batch progress (0-100)</summary>
        property double OverallProgress;

        /// <summary>Current job index (1-based)</summary>
        property int CurrentJobIndex;

        /// <summary>Total number of jobs in batch</summary>
        property int TotalJobs;

        BatchProgressEventArgs(BatchJob^ job, double overallProgress, int currentJobIndex, int totalJobs)
        {
            Job = job;
            OverallProgress = overallProgress;
            CurrentJobIndex = currentJobIndex;
            TotalJobs = totalJobs;
        }
    };

    /// <summary>
    /// Animation keyframe for interpolating between fractal views
    /// </summary>
    public ref class AnimationKeyframe
    {
    public:
        /// <summary>Time position in animation (0.0 = start, 1.0 = end)</summary>
        property double Time;

        /// <summary>Fractal parameters at this keyframe</summary>
        property FractalParameters^ Parameters;

        /// <summary>Optional duration in seconds for this keyframe segment</summary>
        property double Duration;

        AnimationKeyframe()
        {
            Time = 0.0;
            Duration = 1.0;
        }
    };

    /// <summary>
    /// Batch renderer for processing multiple fractal render jobs and creating animations
    /// </summary>
    /// <remarks>
    /// <para>Week 3 Implementation: Queue multiple render jobs, animation frame interpolation, progress events</para>
    /// <para>Supports both single-threaded sequential rendering and animation generation</para>
    /// <para>Example usage:</para>
    /// <code>
    /// var batch = gcnew BatchRenderer();
    /// batch->ProgressChanged += gcnew EventHandler&lt;BatchProgressEventArgs^&gt;(OnProgress);
    /// batch->JobCompleted += gcnew EventHandler&lt;BatchProgressEventArgs^&gt;(OnJobComplete);
    /// 
    /// // Queue individual jobs
    /// auto job1 = batch->AddJob("Frame1", params1, "output1.png");
    /// auto job2 = batch->AddJob("Frame2", params2, "output2.png");
    /// 
    /// // Or create animation
    /// auto keyframes = gcnew List&lt;AnimationKeyframe^&gt;();
    /// // ... add keyframes ...
    /// batch->CreateAnimation("MyAnimation", keyframes, 30, InterpolationMode::EaseInOut);
    /// 
    /// // Process all jobs
    /// batch->ProcessAll();
    /// </code>
    /// </remarks>
    public ref class BatchRenderer
    {
    private:
        List<BatchJob^>^ m_jobs;
        FractalEngineWrapper^ m_engine;
        bool m_isProcessing;
        bool m_cancellationRequested;

        // Internal methods
        void ProcessJobInternal(BatchJob^ job);
        FractalParameters^ InterpolateParameters(FractalParameters^ start, FractalParameters^ end, double t, InterpolationMode mode);
        double ApplyInterpolationCurve(double t, InterpolationMode mode);

    public:
        /// <summary>
        /// Event fired when overall batch progress changes
        /// </summary>
        event EventHandler<BatchProgressEventArgs^>^ ProgressChanged;

        /// <summary>
        /// Event fired when a single job completes (successfully or with error)
        /// </summary>
        event EventHandler<BatchProgressEventArgs^>^ JobCompleted;

        /// <summary>
        /// Event fired when entire batch processing completes
        /// </summary>
        event EventHandler^ BatchCompleted;

        /// <summary>
        /// Creates a new batch renderer
        /// </summary>
        BatchRenderer();

        /// <summary>
        /// Destructor - releases managed resources
        /// </summary>
        ~BatchRenderer();

        /// <summary>
        /// Finalizer - cleanup
        /// </summary>
        !BatchRenderer();

        /// <summary>
        /// Add a single render job to the queue
        /// </summary>
        /// <param name="name">User-friendly name for the job</param>
        /// <param name="parameters">Fractal parameters to render</param>
        /// <param name="outputPath">Output file path (null for in-memory only)</param>
        /// <returns>The created BatchJob</returns>
        BatchJob^ AddJob(String^ name, FractalParameters^ parameters, String^ outputPath);

        /// <summary>
        /// Create animation by interpolating between keyframes
        /// </summary>
        /// <param name="name">Base name for animation frames</param>
        /// <param name="keyframes">List of keyframes to interpolate between (must have at least 2)</param>
        /// <param name="fps">Frames per second (typically 24, 30, or 60)</param>
        /// <param name="mode">Interpolation mode for smooth transitions</param>
        /// <param name="outputDirectory">Directory to save frames (null for in-memory only)</param>
        /// <returns>Number of frames created</returns>
        /// <exception cref="ArgumentException">Thrown if keyframes has less than 2 entries</exception>
        int CreateAnimation(String^ name, List<AnimationKeyframe^>^ keyframes, int fps, InterpolationMode mode, String^ outputDirectory);

        /// <summary>
        /// Process all queued jobs sequentially
        /// </summary>
        /// <remarks>
        /// Fires ProgressChanged events during processing and JobCompleted for each job.
        /// Fires BatchCompleted when all jobs finish.
        /// This is a blocking call - runs on current thread.
        /// </remarks>
        void ProcessAll();

        /// <summary>
        /// Request cancellation of batch processing
        /// </summary>
        /// <remarks>
        /// Processing will stop after current job completes.
        /// Jobs not yet started will be marked as Cancelled.
        /// </remarks>
        void CancelAll();

        /// <summary>
        /// Clear all jobs from queue
        /// </summary>
        /// <param name="includePending">If true, clears pending jobs. If false, only clears completed/failed jobs.</param>
        void ClearJobs(bool includePending);

        /// <summary>
        /// Get list of all jobs in queue
        /// </summary>
        property IReadOnlyList<BatchJob^>^ Jobs
        {
            IReadOnlyList<BatchJob^>^ get() { return m_jobs; }
        }

        /// <summary>
        /// Check if batch is currently processing
        /// </summary>
        property bool IsProcessing
        {
            bool get() { return m_isProcessing; }
        }

        /// <summary>
        /// Get number of pending jobs
        /// </summary>
        property int PendingJobCount
        {
            int get();
        }

        /// <summary>
        /// Get number of completed jobs (successful)
        /// </summary>
        property int CompletedJobCount
        {
            int get();
        }

        /// <summary>
        /// Get number of failed jobs
        /// </summary>
        property int FailedJobCount
        {
            int get();
        }

        /// <summary>
        /// Get overall batch progress (0-100)
        /// </summary>
        property double OverallProgress
        {
            double get();
        }
    };

} // namespace Native
} // namespace ManpCore
