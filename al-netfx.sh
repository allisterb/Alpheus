#!/bin/bash
set -e
MONO_LOG_LEVEL=debug mono ./Alpheus.CommandLine/bin/Debug/Alpheus.CommandLine.exe "$@"
