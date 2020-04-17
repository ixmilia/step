@echo off
setlocal

set thisdir=%~dp0
set configuration=Debug
set runtests=true

:parseargs
if "%1" == "" goto argsdone
if /i "%1" == "-c" goto set_configuration
if /i "%1" == "--configuration" goto set_configuration
if /i "%1" == "-notest" goto set_notest
if /i "%1" == "--notest" goto set_notest

echo Unsupported argument: %1
goto error

:set_configuration
set configuration=%2
shift
shift
goto parseargs

:set_notest
set runtests=false
shift
goto parseargs

:argsdone

:: build
dotnet restore
if errorlevel 1 exit /b 1
dotnet build -c %configuration%
if errorlevel 1 exit /b 1

:: test
if /i "%runtests%" == "true" (
    dotnet test -c %configuration% --no-restore --no-build
    if errorlevel 1 goto error
)
