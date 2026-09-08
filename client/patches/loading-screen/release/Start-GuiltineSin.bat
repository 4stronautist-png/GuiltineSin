@echo off
cd /d "%~dp0"
if exist "%~dp0GuiltineSin-LoadingScreen.ps1" (
    start "" powershell.exe -NoProfile -STA -WindowStyle Hidden -ExecutionPolicy Bypass -File "%~dp0GuiltineSin-LoadingScreen.ps1"
) else (
    start "GuiltineSin" "%~dp0Client_tos_x64.exe" -SERVICE GLOBAL -LANGUAGE English
)
exit /b
