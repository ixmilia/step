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

dotnet restore

:: build schema generator
pushd %~dp0src\IxMilia.Step.SchemaParser
dotnet build -c %configuration%
if errorlevel 1 popd && exit /b 1

:: test schema generator
if /i "%runtests%" == "true" (
    pushd %~dp0src\IxMilia.Step.SchemaParser.Test
    dotnet test -c %configuration%
    if errorlevel 1 popd && exit /b 1
)

:: run schema generator
pushd %~dp0src\IxMilia.Step.SchemaParser
dotnet run
if errorlevel 1 popd && exit /b 1

:: build library
dotnet build -c %configuration%
if errorlevel 1 exit /b 1

:: test library
if /i "%runtests%" == "true" (
    dotnet test -c %configuration% --no-restore --no-build
    if errorlevel 1 exit /b 1
)
