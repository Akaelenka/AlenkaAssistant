# Windows File Blocking Issue - Solution Summary

## Problem
You encountered this error:
```
System.IO.FileLoadException: Could not load file or assembly 
'D:\Applications\Alenka\AlenkaAssistant\AlenkaAssistant\bin\Release\net10.0-windows\AlenkaAssistant.dll'. 
An Application Control policy has blocked this file. (0x800711C7)
```

This occurs because Windows marks downloaded or extracted files with an alternate data stream (`Zone.Identifier`), which triggers Application Control policy blocks.

## Solution Provided

I've created **3 updated unblock scripts** and comprehensive documentation:

### 1. **UnblockAllBuild.ps1** (Recommended - Most Comprehensive)
- Handles both Debug and Release builds
- Unblocks ALL files in the build output (not just DLLs)
- Removes Zone.Identifier streams
- Applies Unblock-File cmdlet for Application Control policy blocks
- Provides detailed progress output

**Usage:**
```powershell
# Unblock Release (default)
.\UnblockAllBuild.ps1

# Unblock both Debug and Release
.\UnblockAllBuild.ps1 -Both

# Unblock Debug specifically
.\UnblockAllBuild.ps1 -Configuration Debug
```

### 2. **UnblockDLLs.ps1** (Updated File-Level)
- Updated to default to Release configuration
- Now unblocks ALL files (not just .dll files)
- Supports `-Debug` switch for Debug configuration
- Shows summary of unblocked files and Zone.Identifier streams removed

**Usage:**
```powershell
# Unblock Release (default)
.\UnblockDLLs.ps1

# Unblock Debug
.\UnblockDLLs.ps1 -Debug
```

### 3. **Scripts/Unblock-DLL.ps1** (Updated Specific DLL)
- Enhanced to handle error 0x800711C7 specifically
- Works for both Release and Debug configurations
- Provides detailed file information and verification
- Better error reporting and next steps

**Usage:**
```powershell
# Unblock Release (default)
.\Scripts\Unblock-DLL.ps1

# Unblock Debug
.\Scripts\Unblock-DLL.ps1 -Configuration Debug
```

### 4. **UnblockBuild.bat** (For Windows Users)
- Easy-to-use batch file wrapper
- Automatically runs PowerShell script with proper permissions
- No need to type PowerShell commands
- Just double-click to run (or right-click → "Run as administrator" for full permissions)

### 5. **UNBLOCK_INSTRUCTIONS.md** (Complete Documentation)
- Detailed explanation of the problem
- All available solutions (quick scripts and manual methods)
- Why Windows blocks these files
- Troubleshooting steps

## How to Resolve Your Current Error

### Quick Fix (Easiest)
1. Right-click `UnblockBuild.bat` in your AlenkaAssistant folder
2. Select "Run as administrator"
3. Wait for the script to complete
4. Run your application

### PowerShell Fix
1. Open PowerShell as Administrator
2. Navigate to: `D:\Applications\Alenka\AlenkaAssistant\`
3. Run: `.\UnblockAllBuild.ps1`

### Manual Fix
1. Right-click on the DLL file: 
   - `D:\Applications\Alenka\AlenkaAssistant\AlenkaAssistant\bin\Release\net10.0-windows\AlenkaAssistant.dll`
2. Click "Properties"
3. At the bottom, click "Unblock" if available
4. Click "Apply" then "OK"

## What These Scripts Do

Both scripts perform two critical operations:

1. **Remove Zone.Identifier Stream**
   - Removes the Windows mark-of-the-web indicator
   - This is a hidden alternate data stream attached to the file

2. **Apply Unblock-File Cmdlet**
   - Removes Application Control policy blocks (error 0x800711C7)
   - Modern Windows security mechanism

## After Unblocking

Your application should now run without the FileLoadException. You can:
- Run via Visual Studio (F5)
- Run the EXE directly: `.\AlenkaAssistant.exe`
- Run via dotnet: `dotnet run`

## If You Still Have Issues

1. **Run as Administrator**
   - Right-click PowerShell and select "Run as administrator"
   - Re-run the unblock script

2. **Rebuild Solution**
   ```bash
   dotnet clean
   dotnet build --configuration Release
   .\UnblockAllBuild.ps1
   ```

3. **Check Event Viewer**
   - Open Windows Event Viewer
   - Navigate to: Applications and Services Logs → Microsoft → Windows → AppLocker
   - Look for details about what's blocking your file

4. **Verify File Properties**
   ```powershell
   Get-Item -Path "path\to\AlenkaAssistant.dll" -Stream Zone.Identifier -ErrorAction SilentlyContinue
   ```
   - If no output appears, the file is unblocked
   - If output appears, the unblocking didn't complete

## Files Created/Updated

- ✅ Created: `AlenkaAssistant/UnblockAllBuild.ps1`
- ✅ Created: `AlenkaAssistant/UnblockBuild.bat`
- ✅ Created: `AlenkaAssistant/UNBLOCK_INSTRUCTIONS.md`
- ✅ Updated: `AlenkaAssistant/UnblockDLLs.ps1` (now defaults to Release, handles all files)
- ✅ Updated: `AlenkaAssistant/Scripts/Unblock-DLL.ps1` (improved error handling, Release support)

All scripts are ready to use!
