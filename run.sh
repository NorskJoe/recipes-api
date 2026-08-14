#!/bin/sh
: "${MSSQL_SA_PASSWORD:?Set MSSQL_SA_PASSWORD in your shell env (see README)}"
export ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=RecipesDB;User Id=sa;Password=${MSSQL_SA_PASSWORD};Encrypt=True;TrustServerCertificate=True;"
dotnet run --project Presentation/Presentation.csproj
