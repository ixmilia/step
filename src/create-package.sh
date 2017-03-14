#!/bin/sh -e

PROJECT=./IxMilia.Step/IxMilia.Step.csproj
dotnet restore $PROJECT
dotnet pack --include-symbols --include-source --configuration Release $PROJECT
