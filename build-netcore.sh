#!/bin/bash

set -e

rm -rf ./Alpheus.CommandLine/bin/Debug/netcoreapp2.0/Examples/*
nuget restore Alpheus.sln && msbuild Alpheus.sln /p:Configuration=CoreD $*