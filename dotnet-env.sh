#!/bin/bash
# .NET 8.0 Environment Configuration
# Source this file before running .NET commands: source dotnet-env.sh

export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"
export DOTNET_ROOT="/opt/homebrew/opt/dotnet@8/libexec"

echo "✓ .NET environment configured"
echo "  .NET version: $(dotnet --version)"
echo "  DOTNET_ROOT: $DOTNET_ROOT"
