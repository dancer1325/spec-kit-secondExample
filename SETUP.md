# Taskify MVP Setup Guide

## Prerequisites Installation

### 1. Install .NET 8.0 SDK

**For macOS (current system: Darwin 25.2.0):**

```bash
# Download and install .NET 8.0 SDK from Microsoft
# Visit: https://dotnet.microsoft.com/download/dotnet/8.0
# Or use Homebrew:
brew install --cask dotnet-sdk
```

**Verify installation:**
```bash
dotnet --version
# Expected output: 8.0.x
```

### 2. Install .NET Aspire Workload

```bash
dotnet workload update
dotnet workload install aspire
```

**Verify Aspire installation:**
```bash
dotnet workload list
# Should show: aspire
```

### 3. Install Docker Desktop (for PostgreSQL container)

Download and install Docker Desktop for Mac:
- Visit: https://www.docker.com/products/docker-desktop

**Verify Docker:**
```bash
docker --version
docker ps
```

## Project Setup Commands

Once prerequisites are installed, run these commands from the repository root:

### Phase 1: Setup (Tasks T001-T009)

```bash
# T001: Create solution file
dotnet new sln -n Taskify

# T002: Create Aspire AppHost project
dotnet new aspire-apphost -o Taskify.AppHost
dotnet sln add Taskify.AppHost/Taskify.AppHost.csproj

# T003: Create ServiceDefaults project
dotnet new aspire-servicedefaults -o Taskify.ServiceDefaults
dotnet sln add Taskify.ServiceDefaults/Taskify.ServiceDefaults.csproj

# T004: Create Blazor Server project
dotnet new blazor -o Taskify.Web --interactivity Server
dotnet sln add Taskify.Web/Taskify.Web.csproj

# T005: Create API Service project
dotnet new webapi -o Taskify.ApiService
dotnet sln add Taskify.ApiService/Taskify.ApiService.csproj

# Add project references
cd Taskify.AppHost
dotnet add reference ../Taskify.ServiceDefaults/Taskify.ServiceDefaults.csproj
dotnet add reference ../Taskify.Web/Taskify.Web.csproj
dotnet add reference ../Taskify.ApiService/Taskify.ApiService.csproj
cd ..

cd Taskify.Web
dotnet add reference ../Taskify.ServiceDefaults/Taskify.ServiceDefaults.csproj
cd ..

cd Taskify.ApiService
dotnet add reference ../Taskify.ServiceDefaults/Taskify.ServiceDefaults.csproj
cd ..

# T006: Add MudBlazor to Blazor project
cd Taskify.Web
dotnet add package MudBlazor
cd ..

# T007: Add Npgsql.EntityFrameworkCore.PostgreSQL to API project
cd Taskify.ApiService
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
cd ..

# T008: Add FluentValidation to API project
cd Taskify.ApiService
dotnet add package FluentValidation.AspNetCore
cd ..

# T009: Create .editorconfig (already created as .gitignore)
# .editorconfig will be created separately for C# formatting
```

### Verify Setup

```bash
# Restore all packages
dotnet restore

# Build solution
dotnet build

# Verify all projects compiled successfully
echo "Setup complete!"
```

## Next Steps After Setup

Once prerequisites are installed and project setup is complete:

1. Run Phase 2 implementation scripts (Foundational tasks T010-T022)
2. Start development with `dotnet run --project Taskify.AppHost`
3. Access application at `http://localhost:5000` (or as specified by Aspire)

## Current Status

- ✅ `.gitignore` created
- ⏳ `.NET 8.0 SDK` - **REQUIRED: Please install**
- ⏳ `.NET Aspire workload` - **REQUIRED: Please install after .NET SDK**
- ⏳ `Docker Desktop` - **REQUIRED: Please install for PostgreSQL**

## Troubleshooting

### .NET SDK not found
```bash
# Check if dotnet is in PATH
which dotnet

# If not found, add to PATH (for Homebrew installation):
export PATH="/usr/local/share/dotnet:$PATH"
echo 'export PATH="/usr/local/share/dotnet:$PATH"' >> ~/.zshrc
```

### Aspire workload installation fails
```bash
# Clear workload cache and reinstall
dotnet workload clean
dotnet workload update
dotnet workload install aspire
```

### Docker not running
```bash
# Start Docker Desktop application
open -a Docker

# Wait for Docker daemon to start, then verify:
docker ps
```
