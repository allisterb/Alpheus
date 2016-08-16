#!/bin/bash
cd ./Alpheus.CommandLine/bin/Debug/
MONO_LOG_LEVEL=debug
mono Alpheus.CommandLine.exe "$@"
cd ../../../
