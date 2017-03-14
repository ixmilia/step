#!/bin/sh -e

TEST_PROJECT=./src/IxMilia.Step.Test/IxMilia.Step.Test.csproj
dotnet restore $TEST_PROJECT
dotnet test $TEST_PROJECT
