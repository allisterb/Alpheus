#!/bin/bash

set -e

cd ./Alpheus.CommandLine/bin/Debug/netcoreapp2.0
dotnet Alpheus.CommandLine.dll "$@"
cd ../../../../
