#!/bin/bash
set -e  # Exit on error

echo "==================================="
echo "Taskify MVP - Phase 2 Foundation"
echo "==================================="
echo ""

# Check if Phase 1 is complete
if [ ! -f "Taskify.sln" ] || [ ! -d "Taskify.ApiService" ] || [ ! -d "Taskify.Web" ]; then
    echo "ERROR: Phase 1 not complete. Please run ./setup-phase1.sh first"
    exit 1
fi

echo "Phase 1 verified ✓"
echo ""
echo "==================================="
echo "Phase 2: Foundational Setup (T010-T022)"
echo "==================================="
echo ""

# Create directory structure
echo "Creating directory structure..."
mkdir -p Taskify.ApiService/Models/Entities
mkdir -p Taskify.ApiService/Models/DTOs
mkdir -p Taskify.ApiService/Models/Requests
mkdir -p Taskify.ApiService/Data
mkdir -p Taskify.ApiService/Services
mkdir -p Taskify.ApiService/Validation
mkdir -p Taskify.ApiService/Controllers
mkdir -p Taskify.ApiService/Hubs
mkdir -p Taskify.Web/Services
mkdir -p Taskify.Web/Models
mkdir -p Tests/Taskify.ApiService.Tests
mkdir -p Tests/Taskify.Web.Tests
mkdir -p Tests/Taskify.E2E.Tests
echo "✓ Directory structure created"

echo ""
echo "Next: Run the C# code generation script to create entities and DbContext"
echo "This script creates the directory structure. Entity models and DbContext"
echo "will be created by the implementation script once .NET is available."
echo ""
echo "Files to be created in Phase 2:"
echo "  - Taskify.ApiService/Models/Entities/User.cs (T012)"
echo "  - Taskify.ApiService/Models/Entities/Project.cs (T013)"
echo "  - Taskify.ApiService/Models/Entities/Task.cs (T014)"
echo "  - Taskify.ApiService/Models/Entities/Comment.cs (T015)"
echo "  - Taskify.ApiService/Data/TaskifyDbContext.cs (T016+T017)"
echo "  - Taskify.ApiService/Program.cs configuration (T021)"
echo "  - Taskify.Web/Services/UserContextService.cs (T019)"
echo "  - Taskify.Web/Services/TaskifyApiClient.cs (T020)"
echo "  - Taskify.Web/Program.cs configuration (T022)"
echo "  - Taskify.AppHost/Program.cs Aspire orchestration (T010)"
echo "  - Taskify.ServiceDefaults/Extensions.cs (T011)"
echo ""
echo "Phase 2 directory structure ready ✓"
