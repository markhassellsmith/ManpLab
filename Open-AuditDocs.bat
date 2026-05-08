@echo off
REM Open Fractal Audit Documentation in Default Editor

echo.
echo Opening Fractal Audit Documentation...
echo.

REM Start the main guide
start "" "Documentation\FractalReviewAudit\START_HERE.md"

REM Wait a moment
timeout /t 2 /nobreak >nul

REM Start the checklist
start "" "Documentation\FractalReviewAudit\Checklists\TIER1_CRITICAL_FRACTALS.md"

echo.
echo Opened:
echo   - START_HERE.md (Guide)
echo   - TIER1_CRITICAL_FRACTALS.md (Checklist)
echo.
echo Ready to start auditing!
echo.
pause
