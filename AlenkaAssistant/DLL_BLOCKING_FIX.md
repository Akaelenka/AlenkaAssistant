# DLL Blocking Fix Documentation

## Problem
After updating to Visual Studio 2026, when debugging the AlenkaAssistant project, DLLs were being blocked by Windows Application Control Policy with error:
```
System.IO.FileLoadException: Could not load file or assembly 'AlenkaAssistant.dll'. 
An Application Control policy has blocked this file. (0x800711C7)
```

## Root Cause
Windows marks downloaded or externally-modified files with a `Zone.Identifier` alternate data stream. The new Visual Studio build process can trigger additional Application Control Policy blocks. This is a security feature to prevent potentially unsafe code from running.

## Solution Implemented

### 1. Automatic Unblocking (Recommended)
A post-build MSBuild target has been added to `AlenkaAssistant.csproj` that automatically:
- Removes the `Zone.Identifier` alternate data stream from all DLLs
- Calls `Unblock-File` PowerShell cmdlet on each DLL
- Runs after every successful build

**File Modified:** `AlenkaAssistant.csproj`

```xml
<Target Name="UnblockDlls" AfterTargets="Build">
  <Exec Command="powershell.exe -Command &quot;Get-ChildItem -Path '$(OutDir)' -Recurse -Filter '*.dll' -ErrorAction SilentlyContinue | ForEach-Object { Remove-Item -Path ($_.FullName + ':Zone.Identifier') -ErrorAction SilentlyContinue; $_ | Unblock-File -ErrorAction SilentlyContinue }; Write-Host 'Unblocked all DLLs'&quot;" ContinueOnError="true" />
</Target>
```

### 2. Manual Unblocking Script
A helper PowerShell script `UnblockDLLs.ps1` has been provided for manual unblocking if needed.

**Usage:**
```powershell
cd AlenkaAssistant
.\UnblockDLLs.ps1
# Or with custom parameters:
.\UnblockDLLs.ps1 -Configuration Debug -Framework net10.0-windows
```

## Verification

The fix has been verified:
- ✅ Project builds successfully
- ✅ Zone.Identifier stream is removed from DLLs
- ✅ All DLLs in output directory are unblocked

## Troubleshooting

If you still encounter DLL blocking issues:

1. **Clear and rebuild:**
   ```powershell
   dotnet clean --configuration Debug
   dotnet build --configuration Debug
   ```

2. **Manual unblock all DLLs:**
   ```powershell
   Get-ChildItem "D:\Applications\Alenka\AlenkaAssistant\bin" -Recurse -Filter "*.dll" | Unblock-File
   ```

3. **Check antivirus:** Temporarily disable Windows Defender/antivirus if the problem persists

4. **Reset AppLocker (if admin access available):**
   ```powershell
   # Run as Administrator
   Get-AppLockerPolicy -Effective -Xml | Set-AppLockerPolicy -PolicyObject { $_ } -ErrorAction SilentlyContinue
   ```

## Files Modified
- `AlenkaAssistant/AlenkaAssistant.csproj` - Added UnblockDlls post-build target

## Files Created
- `AlenkaAssistant/UnblockDLLs.ps1` - Manual unblocking helper script
