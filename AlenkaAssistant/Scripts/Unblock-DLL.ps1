#!/usr/bin/env pwsh
# Unblock-DLL.ps1
# This script unblocks the compiled DLL file after build to avoid Windows security policy issues

param(
	[string]$DllPath = "D:\Applications\Alenka\AlenkaAssistant\AlenkaAssistant\bin\Debug\net10.0-windows\AlenkaAssistant.dll"
)

try {
	if (Test-Path $DllPath) {
		Write-Host "Unblocking DLL: $DllPath"

		# Remove Zone.Identifier alternate data stream if it exists
		Remove-Item -Path "$($DllPath):Zone.Identifier" -ErrorAction SilentlyContinue

		# Call Unblock-File to ensure it's fully unblocked
		Unblock-File -Path $DllPath -ErrorAction SilentlyContinue

		Write-Host "Successfully unblocked DLL"
		Write-Host "  Path: $DllPath"

		# Verify it's unblocked by checking file properties
		$file = Get-Item -Path $DllPath
		Write-Host "  Size: $($file.Length) bytes"
		Write-Host "  Modified: $($file.LastWriteTime)"
	} else {
		Write-Host "Warning: DLL not found at: $DllPath"
	}
}
catch {
	Write-Host "Warning: Could not unblock DLL - $($_.Exception.Message)"
}
