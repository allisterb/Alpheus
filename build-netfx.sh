#!/bin/bash

set -e

rm -rf ./Alpheus.CommandLine/bin/Debug/Examples/*
nuget restore Alpheus.sln && msbuild Alpheus.sln /p:Configuration=Debug $*
