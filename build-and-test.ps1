#!/usr/bin/pwsh

[CmdletBinding(PositionalBinding = $false)]
param (
    [string]$configuration = "Debug",
    [switch]$noTest
)

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

function Fail([string]$message) {
    throw $message
}

function Single([string]$pattern) {
    $items = @(Get-Item $pattern)
    if ($items.Length -ne 1) {
        $itemsList = $items -Join "`n"
        Fail "Expected single item, found`n$itemsList`n"
    }

    return $items[0]
}

try {
    dotnet restore || Fail "Error restoring."
    dotnet build --configuration $configuration || Fail "Error building."
    if (-Not $noTest) {
        dotnet test --no-restore --no-build --configuration $configuration || Fail "Error running tests."
    }
    dotnet pack --no-restore --no-build --configuration $configuration || "Error creating package."
    $package = Single "$PSScriptRoot/artifacts/packages/$configuration/*.nupkg"
    Write-Host "Package generated at '$package'"
}
catch {
    Write-Host $_
    Write-Host $_.Exception
    Write-Host $_.ScriptStackTrace
    exit 1
}
