Write-Host "Starting NexusERP API..."
cd C:\Users\vcald\NexusERP
dotnet run --project src\NexusERP.Api\NexusERP.Api.csproj --urls "http://localhost:5000" -c Release
