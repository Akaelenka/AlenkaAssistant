#!/usr/bin/env pwsh
# Unblock-DLL.ps1
# This script unblocks the compiled DLL file after build to avoid Windows security policy issues
# Updated to handle Application Control policy blocks (error 0x800711C7)

param(
	[string]$DllPath = "D:\Applications\Alenka\AlenkaAssistant\AlenkaAssistant\bin\Release\net10.0-windows\AlenkaAssistant.dll",
	[string]$Configuration = "Release"
)

# Auto-detect path if using Debug configuration
if ($Configuration -eq "Debug") {
	$DllPath = "D:\Applications\Alenka\AlenkaAssistant\AlenkaAssistant\bin\Debug\net10.0-windows\AlenkaAssistant.dll"
}

function Unblock-BuildOutput {
	param(
		[string]$FilePath
	)

	if (-not (Test-Path $FilePath)) {
		Write-Host "⚠ Warning: File not found at: $FilePath" -ForegroundColor Yellow
		return $false
	}

	try {
		Write-Host "🔓 Unblocking file: $FilePath" -ForegroundColor Cyan

		# Step 1: Remove Zone.Identifier alternate data stream
		# This is the "Mark of the Web" indicator that Windows adds to downloaded files
		$ZoneStream = $FilePath + ":Zone.Identifier"
		if (Test-Path $ZoneStream) {
			Remove-Item -Path $ZoneStream -ErrorAction Stop
			Write-Host "  ✓ Removed Zone.Identifier alternate data stream" -ForegroundColor Green
		}

		# Step 2: Call Unblock-File cmdlet
		# This removes Application Control policy blocks and other security restrictions
		# Error 0x800711C7 is typically resolved by this step
		Unblock-File -Path $FilePath -ErrorAction Stop
		Write-Host "  ✓ Applied Unblock-File cmdlet" -ForegroundColor Green

		# Display file information
		$file = Get-Item -Path $FilePath
		Write-Host "`n📊 File Information:" -ForegroundColor Cyan
		Write-Host "  Name: $($file.Name)" -ForegroundColor White
		Write-Host "  Path: $($file.FullName)" -ForegroundColor White
		Write-Host "  Size: $($file.Length) bytes" -ForegroundColor White
		Write-Host "  Modified: $($file.LastWriteTime)" -ForegroundColor White

		# Verify unblocking
		$properties = Get-Item -Path $FilePath -Stream Zone.Identifier -ErrorAction SilentlyContinue
		if ($null -eq $properties) {
			Write-Host "`n✅ File is successfully unblocked!" -ForegroundColor Green
			return $true
		} else {
			Write-Host "`n⚠ File may still have security attributes" -ForegroundColor Yellow
			return $false
		}
	}
	catch {
		Write-Host "❌ Error unblocking file: $($_.Exception.Message)" -ForegroundColor Red
		return $false
	}
}

# Execute unblocking
$success = Unblock-BuildOutput -FilePath $DllPath

if ($success) {
	Write-Host "`n✅ Ready to run the application!" -ForegroundColor Green
	exit 0
} else {
	Write-Host "`n⚠ Unblocking may not have completed successfully. You can try:" -ForegroundColor Yellow
	Write-Host "  1. Run as Administrator" -ForegroundColor White
	Write-Host "  2. Rebuild the solution" -ForegroundColor White
	Write-Host "  3. Check application event log for details" -ForegroundColor White
	exit 1
}

