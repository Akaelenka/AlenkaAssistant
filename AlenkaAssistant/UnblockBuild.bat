@echo off
REM UnblockBuild.bat - Windows batch script to unblock DLL files
REM This script helps users who prefer batch files over PowerShell

setlocal enabledelayedexpansion

echo.
echo ============================================================
echo   AlenkaAssistant Build Output Unblock Tool
echo ============================================================
echo.

REM Get the directory where this script is located
set SCRIPT_DIR=%~dp0

echo Launching PowerShell unblock script...
echo.

REM Try to run the PowerShell script
REM First, check if we're running as Administrator (required for some operations)
net session >nul 2>&1
if %errorlevel% neq 0 (
	echo WARNING: This script should be run as Administrator for best results.
	echo.
	echo To run as Administrator:
	echo   1. Right-click this file
	echo   2. Select "Run as administrator"
	echo.
	echo Continuing with current privileges...
	echo.
)

REM Run the comprehensive unblock script
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "& '%SCRIPT_DIR%UnblockAllBuild.ps1' -Both"

if %errorlevel% equ 0 (
	echo.
	echo ============================================================
	echo   SUCCESS! Files have been unblocked.
	echo ============================================================
	echo.
	echo You can now run your application.
	echo.
) else (
	echo.
	echo ============================================================
	echo   WARNING: Unblocking may not have completed successfully.
	echo ============================================================
	echo.
	echo Try these options:
	echo   1. Run this script as Administrator (right-click, select "Run as administrator")
	echo   2. Close all instances of your application and rebuild
	echo   3. Check the Windows Event Viewer for security policy details
	echo.
)

pause
