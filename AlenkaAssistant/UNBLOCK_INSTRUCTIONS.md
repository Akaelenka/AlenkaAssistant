# How to Unblock Build Output

This directory contains PowerShell scripts to unblock Windows security blocks on your built application files.

## Error You're Experiencing

```
System.IO.FileLoadException: Could not load file or assembly 'AlenkaAssistant.dll'. 
An Application Control policy has blocked this file. (0x800711C7)
```

This error occurs because Windows marks files as "blocked" when they are downloaded or extracted. This is a security feature, but it prevents the application from running.

## Quick Solutions

### Option 1: Use the Comprehensive Unblock Script (Recommended)
```powershell
# Unblock Release build (default)
.\UnblockAllBuild.ps1

# Unblock both Debug and Release
.\UnblockAllBuild.ps1 -Both

# Unblock Debug build
.\UnblockAllBuild.ps1 -Configuration Debug
```

### Option 2: Use the Simple DLL Unblock Script
```powershell
# Unblock Release build (default)
.\Scripts\Unblock-DLL.ps1

# Unblock Debug build
.\Scripts\Unblock-DLL.ps1 -Configuration Debug
```

### Option 3: Use the File-Level Unblock Script
```powershell
# Unblock Release build files
.\UnblockDLLs.ps1

# Unblock Debug build files
.\UnblockDLLs.ps1 -Debug
```

## Manual Unblocking (If Scripts Don't Work)

### Method 1: Using PowerShell (Run as Administrator)
```powershell
$DllPath = "D:\Applications\Alenka\AlenkaAssistant\AlenkaAssistant\bin\Release\net10.0-windows\AlenkaAssistant.dll"

# Remove Zone.Identifier stream
Remove-Item -Path "$($DllPath):Zone.Identifier" -ErrorAction SilentlyContinue

# Unblock the file
Unblock-File -Path $DllPath
```

### Method 2: Using File Properties
1. Right-click on `AlenkaAssistant.dll` in File Explorer
2. Select "Properties"
3. At the bottom, check if there's a checkbox for "Unblock"
4. Click "Unblock" if available, then "Apply" and "OK"

### Method 3: Unblock All Files in Output Directory
```powershell
$BinPath = "D:\Applications\Alenka\AlenkaAssistant\AlenkaAssistant\bin\Release\net10.0-windows"
Get-ChildItem -Path $BinPath -Recurse | Unblock-File -ErrorAction SilentlyContinue
```

## After Unblocking

1. Run the application:
   - Via Visual Studio: Press `F5` or click "Start"
   - Via PowerShell: `.\AlenkaAssistant.exe` from the bin folder
   - Via dotnet: `dotnet run` from the project directory

2. If you still get errors, try:
   - Rebuilding the solution (Clean → Build)
   - Restarting Visual Studio
   - Running PowerShell as Administrator and re-running the unblock script

## Why This Happens

Windows adds an alternate data stream called `Zone.Identifier` to files downloaded from the internet or extracted from archives. The value can be:
- Zone 0 = Local computer
- Zone 1 = Local intranet
- Zone 2 = Trusted sites
- Zone 3 = Internet
- Zone 4 = Restricted sites

When files are marked with Zone 3 or 4, Windows AppLocker and Application Control policies may block execution.

## Technical Details

The unblock scripts perform two key operations:

1. **Remove Zone.Identifier stream**
   ```powershell
   Remove-Item -Path "file.dll:Zone.Identifier"
   ```

2. **Apply Unblock-File cmdlet**
   ```powershell
   Unblock-File -Path "file.dll"
   ```

This combination handles both the legacy zone marker and modern Application Control policies that may block the file.

## Questions or Issues?

If these scripts don't work:
- Ensure you're running PowerShell as Administrator
- Check that the paths are correct for your installation
- Try rebuilding the solution: `dotnet clean && dotnet build --configuration Release`
- Check Windows Event Viewer for Application Control policy violations
