#!/bin/bash

# Define variables (that might or might not be set)
# Without this, the script will fail with an unbound variable" error (because of the 'set -e')
TF_BUILD=$TF_BUILD
GITHUB_ACTIONS=$GITHUB_ACTIONS

set -e

# Group log output of SDK installation on GitHub Actions or Azure Pipelines
if [[ -n "${TF_BUILD}" ]]; then
    echo "##[group]Install .NET SDK"
fi
if [[ "${GITHUB_ACTIONS}" == "true" ]]; then
    echo "::group::Install .NET SDK"
fi

source ./build/dotnet-install.sh --channel "8.0" --runtime dotnet

# Install SDK and runtime as specified in global.json
source ./build/dotnet-install.sh --jsonfile "global.json"

dotnet --info

# End grouping of log output on GitHub Actions or Azure Pipelines
if [[ -n "${TF_BUILD}" ]]; then
    echo "##[endgroup]"
fi
if [[ "${GITHUB_ACTIONS}" == "true" ]]; then
    echo "::endgroup::"
fi

dotnet run --project build/workflow -- $@
