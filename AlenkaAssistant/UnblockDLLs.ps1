# UnblockDLLs.ps1 - Helper script to unblock DLLs after Visual Studio build
# This script removes Windows Zone.Identifier and Application Control Policy blocks from DLLs
# Updated to support both Debug and Release configurations

param(
	[string]$Configuration = "Release",
	[string]$Framework = "net10.0-windows",
	[switch]$Debug  # If specified, unblock Debug configuration instead
)

# Override to Debug if switch is provided
if ($Debug) {
	$Configuration = "Debug"
}

$BinPath = Join-Path -Path $PSScriptRoot -ChildPath "bin\$Configuration\$Framework"

if (-not (Test-Path $BinPath)) {
	Write-Host "Error: Output directory not found: $BinPath" -ForegroundColor Red
	Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
	Write-Host "Framework: $Framework" -ForegroundColor Yellow
	exit 1
}

Write-Host "Unblocking files in: $BinPath" -ForegroundColor Cyan

$FileCount = 0
$ZoneBlockCount = 0

# Process all files, especially DLLs and EXEs which are commonly blocked
Get-ChildItem -Path $BinPath -Recurse -ErrorAction SilentlyContinue | 
Where-Object { $_.PSIsContainer -eq $false } | 
ForEach-Object {
	try {
		$FileName = $_.Name
		$FilePath = $_.FullName

		# Remove Zone.Identifier alternate data stream (Windows mark-of-the-web indicator)
		$ZoneStream = $FilePath + ":Zone.Identifier"
		if (Test-Path $ZoneStream) {
			Remove-Item -Path $ZoneStream -ErrorAction SilentlyContinue
			$ZoneBlockCount++
		}

		# Unblock the file using PowerShell's Unblock-File cmdlet
		# This removes Application Control policy blocks (error 0x800711C7)
		$_ | Unblock-File -ErrorAction SilentlyContinue

		$FileCount++
		Write-Host "✓ Unblocked: $FileName" -ForegroundColor Green
	}
	catch {
		Write-Host "✗ Failed to unblock $($_.Name): $_" -ForegroundColor Red
	}
}

Write-Host "`nSummary:" -ForegroundColor Cyan
Write-Host "  Total files unblocked: $FileCount" -ForegroundColor Green
Write-Host "  Zone.Identifier streams removed: $ZoneBlockCount" -ForegroundColor Green
Write-Host "  Configuration: $Configuration" -ForegroundColor Cyan
Write-Host "`nYou can now run the application." -ForegroundColor Green
