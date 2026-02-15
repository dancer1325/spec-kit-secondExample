#!/bin/bash
set -e

echo "==================================="
echo "Taskify MVP - Phase 2 File Deployment"
echo "==================================="
echo ""

# Check if Phase 1 is complete
if [ ! -f "Taskify.sln" ]; then
    echo "ERROR: Phase 1 not complete. Please run ./setup-phase1.sh first"
    exit 1
fi

echo "Deploying Phase 2 template files to project structure..."
echo ""

# T012-T015: Copy entity models
echo "T012-T015: Deploying entity models..."
cp templates/phase2/User.cs Taskify.ApiService/Models/Entities/
cp templates/phase2/Project.cs Taskify.ApiService/Models/Entities/
cp templates/phase2/Task.cs Taskify.ApiService/Models/Entities/
cp templates/phase2/Comment.cs Taskify.ApiService/Models/Entities/
echo "✓ Entity models deployed"

# T016-T017: Copy DbContext
echo ""
echo "T016-T017: Deploying TaskifyDbContext..."
cp templates/phase2/TaskifyDbContext.cs Taskify.ApiService/Data/
echo "✓ TaskifyDbContext deployed"

# T019: Copy UserContextService
echo ""
echo "T019: Deploying UserContextService..."
cp templates/phase2/UserContextService.cs Taskify.Web/Services/
echo "✓ UserContextService deployed"

# T020: Copy TaskifyApiClient
echo ""
echo "T020: Deploying TaskifyApiClient..."
cp templates/phase2/TaskifyApiClient.cs Taskify.Web/Services/
echo "✓ TaskifyApiClient deployed"

# T010: Copy AppHost Program.cs
echo ""
echo "T010: Deploying AppHost Program.cs..."
cp templates/phase2/AppHost_Program.cs Taskify.AppHost/Program.cs
echo "✓ AppHost Program.cs deployed"

# T011: Copy ServiceDefaults Extensions.cs
echo ""
echo "T011: Deploying ServiceDefaults Extensions.cs..."
cp templates/phase2/ServiceDefaults_Extensions.cs Taskify.ServiceDefaults/Extensions.cs
echo "✓ ServiceDefaults Extensions.cs deployed"

# T021: Copy ApiService Program.cs
echo ""
echo "T021: Deploying ApiService Program.cs..."
cp templates/phase2/ApiService_Program.cs Taskify.ApiService/Program.cs
echo "✓ ApiService Program.cs deployed"

# T022: Copy Web Program.cs
echo ""
echo "T022: Deploying Web Program.cs..."
cp templates/phase2/Web_Program.cs Taskify.Web/Program.cs
echo "✓ Web Program.cs deployed"

# Build the solution to verify everything compiles
echo ""
echo "Building solution to verify compilation..."
dotnet build

# T018: Create EF Core migration
echo ""
echo "T018: Creating EF Core migration..."
cd Taskify.ApiService
dotnet ef migrations add InitialCreate
echo "✓ Initial migration created"
cd ..

echo ""
echo "==================================="
echo "Phase 2 Deployment Complete! ✓"
echo "==================================="
echo ""
echo "Tasks completed:"
echo "  ✓ T010: Aspire AppHost configured"
echo "  ✓ T011: ServiceDefaults extensions implemented"
echo "  ✓ T012: User entity model created"
echo "  ✓ T013: Project entity model created"
echo "  ✓ T014: Task entity model created"
echo "  ✓ T015: Comment entity model created"
echo "  ✓ T016: TaskifyDbContext created"
echo "  ✓ T017: Database seed data configured"
echo "  ✓ T018: EF Core migration created"
echo "  ✓ T019: UserContextService implemented"
echo "  ✓ T020: TaskifyApiClient implemented"
echo "  ✓ T021: ApiService Program.cs configured"
echo "  ✓ T022: Web Program.cs configured"
echo ""
echo "Next steps:"
echo "  1. Review the generated files in each project"
echo "  2. Start the application: dotnet run --project Taskify.AppHost"
echo "  3. Access Aspire dashboard (URL will be shown in console)"
echo "  4. Verify database migrations applied successfully"
echo ""
echo "Phase 1 & 2 are now complete!"
echo "You can proceed to implement User Stories (Phase 3-7) or stop here for review."
echo ""
