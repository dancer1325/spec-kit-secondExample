#!/bin/bash
set -e  # Exit on error

echo "==================================="
echo "Taskify MVP - Phase 1 Setup"
echo "==================================="
echo ""

# Check prerequisites
echo "Checking prerequisites..."
if ! command -v dotnet &> /dev/null; then
    echo "ERROR: .NET SDK not found. Please install .NET 8.0 SDK first."
    echo "Visit: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo "✓ .NET SDK version: $DOTNET_VERSION"

# Check Aspire workload
if ! dotnet workload list | grep -q aspire; then
    echo "ERROR: .NET Aspire workload not installed."
    echo "Run: dotnet workload install aspire"
    exit 1
fi
echo "✓ .NET Aspire workload installed"

# Check Docker
if ! command -v docker &> /dev/null; then
    echo "WARNING: Docker not found. You'll need Docker for PostgreSQL."
    echo "Visit: https://www.docker.com/products/docker-desktop"
fi

echo ""
echo "==================================="
echo "Phase 1: Project Setup (T001-T009)"
echo "==================================="
echo ""

# T001: Create solution
echo "T001: Creating solution..."
if [ ! -f "Taskify.sln" ]; then
    dotnet new sln -n Taskify
    echo "✓ Taskify.sln created"
else
    echo "✓ Taskify.sln already exists"
fi

# T002: Create AppHost
echo ""
echo "T002: Creating Aspire AppHost project..."
if [ ! -d "Taskify.AppHost" ]; then
    dotnet new aspire-apphost -o Taskify.AppHost
    dotnet sln add Taskify.AppHost/Taskify.AppHost.csproj
    echo "✓ Taskify.AppHost created"
else
    echo "✓ Taskify.AppHost already exists"
fi

# T003: Create ServiceDefaults
echo ""
echo "T003: Creating ServiceDefaults project..."
if [ ! -d "Taskify.ServiceDefaults" ]; then
    dotnet new aspire-servicedefaults -o Taskify.ServiceDefaults
    dotnet sln add Taskify.ServiceDefaults/Taskify.ServiceDefaults.csproj
    echo "✓ Taskify.ServiceDefaults created"
else
    echo "✓ Taskify.ServiceDefaults already exists"
fi

# T004: Create Blazor Server
echo ""
echo "T004: Creating Blazor Server project..."
if [ ! -d "Taskify.Web" ]; then
    dotnet new blazor -o Taskify.Web --interactivity Server
    dotnet sln add Taskify.Web/Taskify.Web.csproj
    echo "✓ Taskify.Web created"
else
    echo "✓ Taskify.Web already exists"
fi

# T005: Create API Service
echo ""
echo "T005: Creating API Service project..."
if [ ! -d "Taskify.ApiService" ]; then
    dotnet new webapi -o Taskify.ApiService
    dotnet sln add Taskify.ApiService/Taskify.ApiService.csproj
    echo "✓ Taskify.ApiService created"
else
    echo "✓ Taskify.ApiService already exists"
fi

# Add project references
echo ""
echo "Adding project references..."

cd Taskify.AppHost
dotnet add reference ../Taskify.ServiceDefaults/Taskify.ServiceDefaults.csproj 2>/dev/null || true
dotnet add reference ../Taskify.Web/Taskify.Web.csproj 2>/dev/null || true
dotnet add reference ../Taskify.ApiService/Taskify.ApiService.csproj 2>/dev/null || true
cd ..

cd Taskify.Web
dotnet add reference ../Taskify.ServiceDefaults/Taskify.ServiceDefaults.csproj 2>/dev/null || true
cd ..

cd Taskify.ApiService
dotnet add reference ../Taskify.ServiceDefaults/Taskify.ServiceDefaults.csproj 2>/dev/null || true
cd ..

echo "✓ Project references added"

# T006: Add MudBlazor
echo ""
echo "T006: Adding MudBlazor package..."
cd Taskify.Web
dotnet add package MudBlazor --version 7.0.0 || dotnet add package MudBlazor
cd ..
echo "✓ MudBlazor added to Taskify.Web"

# T007: Add Npgsql
echo ""
echo "T007: Adding Npgsql.EntityFrameworkCore.PostgreSQL..."
cd Taskify.ApiService
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0 || dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0 || dotnet add package Microsoft.EntityFrameworkCore.Design
cd ..
echo "✓ Npgsql packages added to Taskify.ApiService"

# T008: Add FluentValidation
echo ""
echo "T008: Adding FluentValidation..."
cd Taskify.ApiService
dotnet add package FluentValidation.AspNetCore --version 11.3.0 || dotnet add package FluentValidation.AspNetCore
cd ..
echo "✓ FluentValidation added to Taskify.ApiService"

# T009: Already done (.editorconfig created)
echo ""
echo "T009: ✓ .editorconfig already created"

# Restore and build
echo ""
echo "==================================="
echo "Restoring and building solution..."
echo "==================================="
dotnet restore
dotnet build

echo ""
echo "==================================="
echo "Phase 1 Setup Complete! ✓"
echo "==================================="
echo ""
echo "Tasks completed:"
echo "  ✓ T001: Solution created"
echo "  ✓ T002: AppHost project created"
echo "  ✓ T003: ServiceDefaults project created"
echo "  ✓ T004: Blazor Server project created"
echo "  ✓ T005: API Service project created"
echo "  ✓ T006: MudBlazor package added"
echo "  ✓ T007: Npgsql packages added"
echo "  ✓ T008: FluentValidation package added"
echo "  ✓ T009: .editorconfig configured"
echo ""
echo "Next steps:"
echo "  1. Review the project structure"
echo "  2. Run ./setup-phase2.sh to create entity models and database setup"
echo "  3. Start development with: dotnet run --project Taskify.AppHost"
echo ""
