# Quick Debug Commands for Exception Capture

## If Exception Still Occurs:

### 1. When Debugger Breaks, Run These Commands in Immediate Window:

```csharp
// Get exception details
$exception.GetType().FullName
$exception.Message
$exception.StackTrace
$exception.InnerException?.Message
```

### 2. Copy Call Stack Window
- View → Call Stack (Ctrl+Alt+C)
- Right-click → Select All
- Right-click → Copy
- Paste into chat

### 3. Get Debug Output Logs
In PowerShell:

```powershell
# View last 100 lines of debug output (if you can see Output window)
# Just copy from Output → Debug dropdown in Visual Studio
```

### 4. Check Service State in Watch Window
Add these expressions to Watch window (Debug → Windows → Watch → Watch 1):

```
_fractalParameterService
_fractalParameterService._initialized
_fractalParameterService._parameterTemplates.Count
CurrentParameters
ViewModel.CurrentParameters
```

### 5. Get Fractal That Caused Exception

In Immediate Window:
```csharp
e.Fractal.Name
metadata.Name
ViewModel.SelectedFractalType
```

---

## Quick Test Commands (PowerShell)

### Check if files were modified:
```powershell
cd C:\Users\Mark\source\repos\ManpLab
git status
git diff ManpWinUI/ViewModels/MainViewModel.Parameters.cs
git diff ManpWinUI/Views/MainPage.cs
```

### View recent debug logs (if app has run):
```powershell
$logPath = "$env:LOCALAPPDATA\ManpWinUI\logs"
if (Test-Path $logPath) {
    Get-ChildItem $logPath -Filter "app*.log" | 
    Sort-Object LastWriteTime -Descending | 
    Select-Object -First 1 | 
    Get-Content -Tail 50
}
```

### Check build output:
```powershell
cd C:\Users\Mark\source\repos\ManpLab
dotnet build ManpWinUI\ManpWinUI.csproj 2>&1 | Select-Object -Last 20
```

---

## Expected Success Output Pattern

```
[MainPage] Loading fractal: Mandelbrot
[MainPage] Got metadata for 'Mandelbrot' - Center: (-0.5, 0), Zoom: 1
[OnSelectedFractalTypeChanged] Fractal type changed to: Mandelbrot
[MainViewModel.Parameters] Initializing parameters for 'Mandelbrot'
[FractalParameterService] Initialized with 24 parameter templates
[MainViewModel.Parameters] Loaded 12 parameters for 'Mandelbrot'
[MainPage] Loading parameter editor from flexible system (12 parameters)
[ParameterEditorViewModel] Loading from parameter set: Mandelbrot
[ParameterEditorViewModel] Loaded 15 parameter UI items from flexible system
```

---

## Common Exception Patterns to Watch For

### Pattern 1: NullReferenceException in InitializeParametersForFractal
**Indicates:** Service still null somehow (DI issue)
**Look for:** `Object reference not set to an instance of an object`

### Pattern 2: NullReferenceException in LoadFromParameterSet
**Indicates:** CurrentParameters is null when it shouldn't be
**Look for:** Exception in `ParameterEditorViewModel.LoadFromParameterSet`

### Pattern 3: InvalidOperationException
**Indicates:** Service not ready despite initialization call
**Look for:** `Collection was modified` or `Operation is not valid`

### Pattern 4: ArgumentException
**Indicates:** Bad parameter data (invalid fractal name, etc.)
**Look for:** `The parameter is incorrect`

---

## If You Need More Help

1. Start app (F5)
2. Click fractal
3. If debugger breaks:
   - Copy exception message
   - Copy call stack
   - Run immediate window commands above
   - Copy debug output (last 50 lines)
4. Paste ALL of the above into chat
5. I'll analyze and provide next fix

**Remember:** The fix should prevent the exception, but if you still see it, 
the detailed logs will tell us exactly what's wrong!
