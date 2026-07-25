# UnblockDLLs.ps1 - Helper script to unblock DLLs after Visual Studio build
# This script removes Windows Zone.Identifier and Application Control Policy blocks from DLLs

param(
	[string]$Configuration = "Debug",
	[string]$Framework = "net10.0-windows"
)

$BinPath = Join-Path -Path $PSScriptRoot -ChildPath "bin\$Configuration\$Framework"

if (-not (Test-Path $BinPath)) {
	Write-Host "Error: Output directory not found: $BinPath" -ForegroundColor Red
	exit 1
}

Write-Host "Unblocking DLLs in: $BinPath" -ForegroundColor Cyan

$DllCount = 0
Get-ChildItem -Path $BinPath -Recurse -Filter "*.dll" -ErrorAction SilentlyContinue | ForEach-Object {
	try {
		# Remove Zone.Identifier alternate data stream
		Remove-Item -Path ($_.FullName + ":Zone.Identifier") -ErrorAction SilentlyContinue

		# Unblock the file
		$_ | Unblock-File -ErrorAction SilentlyContinue

		$DllCount++
		Write-Host "✓ Unblocked: $($_.Name)" -ForegroundColor Green
	}
	catch {
		Write-Host "✗ Failed to unblock $($_.Name): $_" -ForegroundColor Red
	}
}

Write-Host "`nSummary: Unblocked $DllCount DLL(s)" -ForegroundColor Cyan
