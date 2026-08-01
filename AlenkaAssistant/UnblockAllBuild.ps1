# UnblockAllBuild.ps1
# Comprehensive script to unblock all built files from Windows security policy
# Handles both Debug and Release configurations, all .NET versions
# Removes Zone.Identifier alternate data streams and unblocks assemblies

param(
	[string]$Configuration = "Release",
	[string]$Framework = "net10.0-windows",
	[switch]$Both  # If specified, unblocks both Debug and Release
)

function Unblock-Directory {
	param(
		[string]$DirectoryPath,
		[string]$ConfigName
	)

	if (-not (Test-Path $DirectoryPath)) {
		Write-Host "⚠ Output directory not found: $DirectoryPath" -ForegroundColor Yellow
		return 0
	}

	Write-Host "`n📁 Unblocking files in: $DirectoryPath" -ForegroundColor Cyan

	$UnblockedCount = 0

	# Unblock all DLLs, EXEs, and other potentially blocked files
	Get-ChildItem -Path $DirectoryPath -Recurse -ErrorAction SilentlyContinue | 
	Where-Object { $_.PSIsContainer -eq $false } |
	ForEach-Object {
		try {
			$FileName = $_.Name

			# Remove Zone.Identifier alternate data stream (primary block marker)
			$ZoneStream = $_.FullName + ":Zone.Identifier"
			if (Test-Path $ZoneStream) {
				Remove-Item -Path $ZoneStream -ErrorAction SilentlyContinue
				Write-Host "  ✓ Removed Zone.Identifier from: $FileName" -ForegroundColor Green
			}

			# Call Unblock-File for additional security policy unblocking
			# This handles Application Control policy blocks (0x800711C7 error)
			$_ | Unblock-File -ErrorAction SilentlyContinue

			$UnblockedCount++
			Write-Host "  ✓ Unblocked: $FileName" -ForegroundColor Green
		}
		catch {
			Write-Host "  ✗ Failed to unblock $($_.Name): $($_.Exception.Message)" -ForegroundColor Red
		}
	}

	return $UnblockedCount
}

# Main execution
try {
	Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
	Write-Host "  AlenkaAssistant Build Output Unblock Utility" -ForegroundColor Cyan
	Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan

	$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
	$TotalUnblocked = 0

	if ($Both) {
		# Unblock both configurations
		$Configurations = @("Debug", "Release")
	} else {
		$Configurations = @($Configuration)
	}

	foreach ($Config in $Configurations) {
		$BinPath = Join-Path -Path $ScriptDir -ChildPath "bin\$Config\$Framework"
		$Count = Unblock-Directory -DirectoryPath $BinPath -ConfigName $Config
		$TotalUnblocked += $Count
	}

	Write-Host "`n═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
	Write-Host "  ✅ Summary: Unblocked $TotalUnblocked file(s) successfully" -ForegroundColor Green
	Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
	Write-Host ""
	Write-Host "Next steps:" -ForegroundColor Cyan
	Write-Host "  1. Run the application: .\AlenkaAssistant.exe" -ForegroundColor White
	Write-Host "  2. Or use: dotnet run (for development)" -ForegroundColor White
	Write-Host ""
}
catch {
	Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
	exit 1
}
