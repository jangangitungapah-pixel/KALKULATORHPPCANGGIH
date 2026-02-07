$ErrorActionPreference = "Stop"

dotnet restore
dotnet build -c Release --no-restore
dotnet test -c Release --no-build
