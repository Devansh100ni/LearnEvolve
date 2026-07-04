# LearnEvolve 🚀

A sample **ASP.NET Core 8 Web API** project created to learn and demonstrate the basics of **[Evolve](https://evolve-db.netlify.app/)**, a database schema migration tool inspired by Flyway.

---

## 📖 Overview

Database migrations are essential for managing database schema changes across different environments in a structured, version-controlled manner. This project demonstrates how to integrate and use **Evolve** within a .NET 8 application using **SQL Server** to automatically execute schema migrations on application startup.

### Why Evolve?
- **Flyway-inspired**: Uses simple SQL scripts for migrations.
- **Cross-database support**: Works seamlessly with SQL Server, PostgreSQL, MySQL, SQLite, Cassandra, and CockroachDB.
- **Embedded or Filesystem scripts**: Supports loading migration scripts directly from embedded resources or external folders.
- **Safe & Reliable**: Automatically tracks applied migrations in a custom metadata table (`_EvolveMigrations`) to prevent re-running scripts.

---

## 📂 Project Structure

```text
LearnEvolve/
├── WebApplication1.sln
└── WebApplication1/
    ├── Controllers/           # API Controllers
    ├── Migrations/            # Evolve SQL Migration Scripts
    │   ├── V1_0_0_1__InitialStage.sql
    │   ├── V1_0_0_2__CreateUserTable.sql
    │   └── V1_0_0_3__SeedUsers.sql
    ├── Program.cs             # Application entry point & Evolve setup
    ├── WebApplication1.csproj # Project configuration & embedded resources
    └── appsettings.json       # Database connection strings
```

---

## 🛠️ Key Concepts & Implementation

### 1. Script Naming Convention
Evolve identifies and orders migration scripts based on their filenames. Scripts must follow strict naming rules:
```text
V<version>__<description>.sql
```
- **`V`**: Prefix indicating a versioned migration.
- **`<version>`**: Version number separated by underscores or dots (e.g., `1_0_0_1`, `2.0`). Evolve executes scripts in ascending version order.
- **`__`** *(double underscore)*: Separator between the version and the description.
- **`<description>`**: Readable description of the migration (e.g., `CreateUserTable`).

### 2. Embedding Migration Scripts
In `WebApplication1.csproj`, SQL scripts located in the `Migrations/` directory are configured as **Embedded Resources** so Evolve can discover them within the compiled assembly:

```xml
<ItemGroup>
    <EmbeddedResource Include="Migrations\V1_0_0_1__InitialStage.sql" />
    <EmbeddedResource Include="Migrations\V1_0_0_2__CreateUserTable.sql" />
    <EmbeddedResource Include="Migrations\V1_0_0_3__SeedUsers.sql" />
</ItemGroup>
```

### 3. Programmatic Migration on Startup
In `Program.cs`, Evolve is initialized with a SQL connection and configured to run before the web application starts receiving requests:

```csharp
using EvolveDb;
using System.Data.SqlClient;

try
{
    var cnx = new SqlConnection(builder.Configuration.GetConnectionString("Default"));
    var evolve = new Evolve(cnx, msg => Console.WriteLine(msg))
    {
        EmbeddedResourceAssemblies = new[] { typeof(Program).Assembly },
        EmbeddedResourceFilters = new[] { "WebApplication1.Migrations" },
        IsEraseDisabled = true, // Prevents accidental database erasing in production
        MetadataTableName = "_EvolveMigrations" // Custom table to track migrations
    };

    evolve.Migrate(); // Executes pending migrations
}
catch (Exception ex)
{
    Console.WriteLine("Database migration failed: " + ex.Message);
    throw;
}
```

---

## 🚀 Getting Started

### Prerequisites
- **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
- **SQL Server** (or SQL Server Express / LocalDB)

### 1. Configure the Database Connection
Open `WebApplication1/appsettings.json` and update the `ConnectionStrings:Default` value to point to your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "Default": "Server=YOUR_SERVER_NAME;Database=LearnEvolveTool;Trusted_Connection=True;TrustServerCertificate=true"
  }
}
```

### 2. Build and Run the Application
You can run the project using the CLI or Visual Studio:

```bash
cd WebApplication1
dotnet run
```

When the application starts:
1. Evolve connects to your SQL Server database (`LearnEvolveTool`).
2. It checks for or creates the `_EvolveMigrations` tracking table.
3. It scans embedded resources for any migration scripts that haven't been applied yet.
4. It executes pending scripts in sequential version order (`V1_0_0_1` → `V1_0_0_2` → `V1_0_0_3`).
5. Swagger UI will be available at `https://localhost:<port>/swagger` in Development mode.

---

## ➕ How to Add a New Migration

To evolve the database schema further:
1. Create a new `.sql` file in the `WebApplication1/Migrations/` directory following the naming convention (e.g., `V1_0_0_4__AddUserPhoneNumber.sql`).
2. Write your standard SQL DDL/DML statements inside the file:
   ```sql
   ALTER TABLE [dbo].[Users] ADD [PhoneNumber] NVARCHAR(20) NULL;
   ```
3. Ensure the script is registered as an `<EmbeddedResource>` in `WebApplication1.csproj`.
4. Restart the application. Evolve will detect the new version and apply only the changes!

---

## 📚 References
- [Evolve Official Documentation](https://evolve-db.netlify.app/)
- [Evolve GitHub Repository](https://github.com/lecaillon/Evolve)
