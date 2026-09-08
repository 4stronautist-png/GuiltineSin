param(
    [string]$Destination = "C:\GuiltineSin"
)

$ErrorActionPreference = "Stop"

$launcher = Join-Path $Destination "release\Start-GuiltineSin.bat"

if (-not (Test-Path -LiteralPath $launcher)) {
    throw "Launcher nao encontrado em $launcher. Rode primeiro .\client\tools\Install-GuiltineSin-Local.ps1"
}

Start-Process -FilePath $launcher
