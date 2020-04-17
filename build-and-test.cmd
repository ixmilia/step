@echo off
setlocal

set thisdir=%~dp0
set configuration=Debug
set runtests=true

:parseargs
if "%1" == "" goto argsdone
if /i "%1" == "-c" (
    set configuration=%2
    shift
    shift
    goto parseargs
)
if /i "%1" == "-notest" (
    set runtests=false
    shift
    goto parseargs
)

echo Unsupported argument: %1
goto error

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
