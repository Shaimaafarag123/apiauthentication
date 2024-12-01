to  create database sql Using EF Core Migrations
1-Install Required NuGet Packages

Ensure the following NuGet packages are installed in your ASP.NET Core project:

Microsoft.EntityFrameworkCore.SqlServer (for SQL Server)
Microsoft.EntityFrameworkCore.Tools (for migrations)

2-Configure the Connection String in appsettings.json

Make sure your connection string is correctly configured in the appsettings.json file


3-Configure the DbContext in Program.cs (or Startup.cs)


4-Create the DbContext Class (AppDbContext)

5-Run Migrations to Create the Database
-Open the terminal in VS Code.

->add-migration+ nameof migration
->upadte-database