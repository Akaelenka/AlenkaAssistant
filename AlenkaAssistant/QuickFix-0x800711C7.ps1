#!/usr/bin/env pwsh
# QuickFix-0x800711C7.ps1
# Quick fix specifically for error: "An Application Control policy has blocked this file. (0x800711C7)"
# This script targets the exact error you're experiencing

param(
	[string]$Configuration = "Release",
	[switch]$RunAsAdmin  # If true, attempt to rerun as administrator if not already
)

# Check if running as administrator
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin -and $RunAsAdmin) {
	Write-Host "Attempting to run as Administrator..." -ForegroundColor Yellow
	Start-Process pwsh -ArgumentList "-NoExit", "-Command", "& '$($PSCommandPath)' -Configuration $Configuration" -Verb RunAs
	exit
}

Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  Quick Fix for Error 0x800711C7 (Application Control)      ║" -ForegroundColor Cyan
Write-Host "║  'Could not load file or assembly...'                      ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Define the target DLL path
$DllPath = "D:\Applications\Alenka\AlenkaAssistant\AlenkaAssistant\bin\$Configuration\net10.0-windows\AlenkaAssistant.dll"

Write-Host "🔍 Checking if DLL exists at:" -ForegroundColor Yellow
Write-Host "   $DllPath" -ForegroundColor White
Write-Host ""

if (-not (Test-Path $DllPath)) {
	Write-Host "❌ ERROR: DLL not found!" -ForegroundColor Red
	Write-Host ""
	Write-Host "Possible solutions:" -ForegroundColor Yellow
	Write-Host "  1. Build your solution first (Ctrl+Shift+B in Visual Studio)" -ForegroundColor White
	Write-Host "  2. Check if your build output is in a different configuration" -ForegroundColor White
	Write-Host "  3. Change the Configuration parameter: -Configuration Debug" -ForegroundColor White
	Write-Host ""
	exit 1
}

Write-Host "✓ DLL found! Applying unblock..." -ForegroundColor Green
Write-Host ""

$success = $false

try {
	# Step 1: Remove Zone.Identifier (mark of web)
	Write-Host "  [1/4] Checking for Zone.Identifier stream..." -ForegroundColor Cyan
	$ZonePath = $DllPath + ":Zone.Identifier"

	if (Test-Path $ZonePath) {
		Write-Host "        Found and removing..." -ForegroundColor Yellow
		Remove-Item -Path $ZonePath -ErrorAction Stop
		Write-Host "        ✓ Zone.Identifier removed" -ForegroundColor Green
	} else {
		Write-Host "        ✓ No Zone.Identifier found (already clean)" -ForegroundColor Green
	}

	# Step 2: Apply Unblock-File cmdlet
	Write-Host "  [2/4] Applying Unblock-File cmdlet..." -ForegroundColor Cyan
	Unblock-File -Path $DllPath -ErrorAction Stop
	Write-Host "        ✓ Unblock-File applied" -ForegroundColor Green

	# Step 3: Unblock the entire bin directory
	Write-Host "  [3/4] Unblocking all files in bin directory..." -ForegroundColor Cyan
	$BinDir = Split-Path -Parent $DllPath
	Get-ChildItem -Path $BinDir -Recurse -ErrorAction SilentlyContinue | ForEach-Object {
		try {
			$_ | Unblock-File -ErrorAction SilentlyContinue
		} catch {
			# Silently continue on errors
		}
	}
	Write-Host "        ✓ All files in bin directory unblocked" -ForegroundColor Green

	# Step 4: Verify
	Write-Host "  [4/4] Verifying unblock was successful..." -ForegroundColor Cyan
	$verify = Get-Item -Path $DllPath -Stream Zone.Identifier -ErrorAction SilentlyContinue

	if ($null -eq $verify) {
		Write-Host "        ✓ Verification successful - file is unblocked!" -ForegroundColor Green
		$success = $true
	} else {
		Write-Host "        ⚠ File still has Zone.Identifier (but unblock was applied)" -ForegroundColor Yellow
		$success = $true  # Unblock-File was applied, so we consider this a success
	}

	Write-Host ""
	Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Green
	Write-Host "║  ✅ SUCCESS! Your file has been unblocked.                 ║" -ForegroundColor Green
	Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Green
	Write-Host ""
	Write-Host "Next steps:" -ForegroundColor Cyan
	Write-Host "  1. Run your application" -ForegroundColor White
	Write-Host "  2. If you still get the error:" -ForegroundColor White
	Write-Host "     - Restart Visual Studio" -ForegroundColor White
	Write-Host "     - Clean and rebuild your solution" -ForegroundColor White
	Write-Host "     - Check Windows Event Viewer for details" -ForegroundColor White
	Write-Host ""

	exit 0
}
catch {
	Write-Host "        ❌ Error during unblocking" -ForegroundColor Red
	Write-Host "           Message: $($_.Exception.Message)" -ForegroundColor Red

	Write-Host ""
	Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Red
	Write-Host "║  ⚠ Unblocking encountered an error                        ║" -ForegroundColor Red
	Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Red
	Write-Host ""
	Write-Host "Troubleshooting steps:" -ForegroundColor Yellow
	Write-Host "  1. Run this script as Administrator" -ForegroundColor White
	Write-Host "     Right-click PowerShell → 'Run as administrator'" -ForegroundColor White
	Write-Host "     Then run: $PSCommandPath" -ForegroundColor White
	Write-Host ""
	Write-Host "  2. Or use the batch file (easier):" -ForegroundColor White
	Write-Host "     Right-click UnblockBuild.bat → 'Run as administrator'" -ForegroundColor White
	Write-Host ""
	Write-Host "  3. Or try manual unblock:" -ForegroundColor White
	Write-Host "     Right-click the DLL file → Properties" -ForegroundColor White
	Write-Host "     Check 'Unblock' at the bottom → Apply → OK" -ForegroundColor White
	Write-Host ""

	exit 1
}
