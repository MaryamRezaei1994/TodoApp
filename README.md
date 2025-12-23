# TodoApp (Clean Architecture) - .NET 8

This is a minimal Clean Architecture scaffold for a Todo list Web API using .NET 8, EF Core, and Swagger.

Structure:
- Domain: Entities and repository interfaces
- Application: DTOs and services
- Infrastructure: EF Core DbContext and repository implementation
- WebApi: Controllers and Program.cs

Getting started

1. Install .NET 8 SDK (if not already installed).
2. Update the connection string in `appsettings.json`.
3. Restore packages and run:

```powershell
cd "e:\Rezaei\Backend\New folder\New folder\TodoApp"
dotnet restore
dotnet ef database update # optional - requires tools and configured DB
dotnet run
```

4. Open Swagger UI: https://localhost:5001/swagger (port shown in console)

APIs (Swagger will list them):
- GET /api/todos
- GET /api/todos/{id}
- POST /api/todos
- PUT /api/todos/{id}
- DELETE /api/todos/{id}
- PATCH /api/todos/{id}/toggle

Notes
- This scaffold uses SQL Server LocalDB by default (`(localdb)\\mssqllocaldb`). Change to your DB server in `appsettings.json`.
- Add migrations with `dotnet ef migrations add Initial` and apply with `dotnet ef database update`.
- You can extend the Application layer with MediatR or validation as needed.
