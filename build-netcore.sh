#!/bin/bash

set -e

rm -rf ./Alpheus.CommandLine/bin/Debug/netcoreapp2.0/Examples/*
dotnet restore Alpheus.sln && dotnet build Alpheus.sln /p:Configuration=CoreD $*