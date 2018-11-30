#!/bin/sh -e

_SCRIPT_DIR="$( cd -P -- "$(dirname -- "$(command -v -- "$0")")" && pwd -P )"
PROJECT_NAME=IxMilia.Step
CONFIGURATION=Release
PROJECT=$_SCRIPT_DIR/$PROJECT_NAME/$PROJECT_NAME.csproj

dotnet restore "$PROJECT"
dotnet build "$PROJECT" --configuration $CONFIGURATION
dotnet pack --no-restore --no-build --configuration $CONFIGURATION "$PROJECT"
