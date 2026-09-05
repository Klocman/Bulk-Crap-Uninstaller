#!/usr/bin/env bash
# EBUninstaller Pro - Cross-platform build & verification runner
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"
BUILD_DIR="$REPO_ROOT/build"

echo "================================================================="
echo " EBUninstaller Pro - Build & Quality Verification Runner         "
echo "================================================================="

mkdir -p "$BUILD_DIR/portable" "$BUILD_DIR/installer"

if command -v dotnet &> /dev/null; then
    echo "[1/3] Building solution with dotnet CLI..."
    dotnet build "$REPO_ROOT/source/BulkCrapUninstaller.sln" -c Release
    
    echo "[2/3] Running tests..."
    dotnet test "$REPO_ROOT/source/BulkCrapUninstallerTests/BulkCrapUninstallerTests.csproj" -c Release --no-build
else
    echo "[INFO] dotnet CLI not found in current environment. Running static verification & integrity checks..."
fi

echo "[3/3] Running syntax and repository integrity analysis..."
python3 "$REPO_ROOT/scripts/verify_repo.py"

echo "Verification completed successfully!"
