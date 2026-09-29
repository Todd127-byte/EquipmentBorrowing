# Campus Equipment Borrowing System

## 1. Solution Structure

This project is a C#/.NET implementation of a Campus Equipment Borrowing
System. The solution separates responsibilities into different projects.

### EquipmentBorrowing.Domain

The Domain project contains the main concepts of the equipment borrowing
system.

It contains:

- Student
- Equipment
- Borrowing
- BorrowingStatus

These classes represent information and state belonging to the problem domain.

### EquipmentBorrowing.Application

The Application project contains the application's business operations.

It contains:

- Repository interfaces
- BorrowEquipmentService

The `BorrowEquipmentService` coordinates the borrowing process and checks
the required business rules before creating a borrowing.

### EquipmentBorrowing.Infrastructure

The Infrastructure project contains the concrete repository implementations.

For this laboratory activity, the repositories use in-memory C# collections
instead of a database.

It contains:

- InMemoryStudentRepository
- InMemoryEquipmentRepository
- InMemoryBorrowingRepository

### EquipmentBorrowing.Tests

The Tests project is intended for automated tests of the application's
behavior.

### EquipmentBorrowing.Demo

The Demo project is a small console application used to demonstrate the
borrowing use case.

It demonstrates both a successful borrowing and an unsuccessful borrowing.

---

## 2. Dependency Direction

The application uses repository interfaces instead of directly depending
on concrete data-storage implementations.

The current dependency direction is:

```text
EquipmentBorrowing.Demo
        |
        v
EquipmentBorrowing.Application
        |
        v
EquipmentBorrowing.Domain

EquipmentBorrowing.Infrastructure
        |
        +----> EquipmentBorrowing.Application
        |
        +----> EquipmentBorrowing.Domain
```

## 3. Desktop Project

`EquipmentBorrowing.Desktop` is the Avalonia desktop interface for the system. It presents equipment, lets a user submit a borrowing, lists active borrowings, and records returns. Its Views bind to ViewModels; the ViewModels call Application services. The Desktop project is the composition root that registers the existing Infrastructure repositories and Application services with dependency injection. It references Application and Infrastructure to assemble the running application, while neither layer references the UI.

## 4. Updated Architecture

```text
Avalonia View
    ↓ data binding and commands
ViewModel
    ↓
Application Service
    ↓
Repository Interface
    ↓
Infrastructure Implementation
```

The Domain project remains independent of Avalonia. It defines the system's entities and state transitions, including students, equipment, and borrowings. Application services coordinate use cases and enforce application rules using repository interfaces. Infrastructure supplies the in-memory implementations. The Desktop project wires these pieces together and displays their results.

## 5. Borrow Equipment Flow

```text
User Action
    ↓
Equipment View
    ↓ command
EquipmentViewModel
    ↓
BorrowEquipmentService
    ↓
Student, equipment, and borrowing repository interfaces / Domain
    ↓
Result
    ↓
EquipmentViewModel
    ↓
Updated status message and equipment list
```

The ViewModel checks that the required form values were selected and passes their identifiers and the expected return date to `BorrowEquipmentService`. The service checks the student, availability, active borrowing limit, and date, then records the borrowing and marks the equipment unavailable. The ViewModel shows a success or readable failure message and reloads the inventory.

## 6. Return Equipment Flow

```text
User selects an active borrowing and chooses Return Equipment
    ↓
Borrowings View command
    ↓
BorrowingsViewModel
    ↓
ReturnEquipmentService
    ↓
Borrowing and equipment repository interfaces / Domain
    ↓
Borrowing marked returned; equipment marked available
    ↓
ViewModel reloads active borrowings and equipment
    ↓
Updated lists and result message
```

The service looks up the borrowing, rejects missing or already returned records, finds the equipment, and updates both domain states through the repository interfaces. The Desktop views do not modify repository data.

## 7. Architectural Reflection

### Why should the View not call a repository directly?

The View should describe the interface and bind to data. Calling repositories from a View mixes storage work into presentation code and makes the interface harder to change or test. The ViewModel and Application service provide the intended path to data.

### Why should business rules not be implemented in the ViewModel?

Rules in a ViewModel would be tied to one interface. The same borrowing rules need to apply to any caller, including the console Demo or a future interface. Application services and Domain objects can enforce them consistently.

### What is the responsibility of the ViewModel?

The ViewModel exposes observable data and commands for the View, checks that required form values are present, calls Application services, and turns their results into UI state and feedback messages.

### Why can the Application layer work without knowing Avalonia is being used?

Application depends on Domain types and repository interfaces, not on controls or Avalonia packages. That keeps its use cases usable by the console Demo, tests, or a different UI.

### What advantage comes from registering dependencies in one composition point?

The Desktop startup code shows which implementations are used and how they are connected. ViewModels receive their dependencies through constructors instead of creating their own repositories or services, and the shared in-memory repositories keep state while the user navigates.

### If in-memory storage were replaced by SQLite, which interface parts should remain largely unchanged?

The Views, ViewModels, Application services, and repository interfaces should remain largely unchanged. Infrastructure would provide SQLite-backed implementations of those interfaces, and the composition root would register the new implementations.

## Laboratory Activity 3 – Persistent Storage

The preceding sections preserve the Activity 1 and Activity 2 documentation of the original in-memory implementation. For Activity 3, the Desktop application uses EF Core and SQLite behind the same repository interfaces. Avalonia Views and ViewModels still call Application services and do not access a DbContext.

### Relational Database Design

See [docs/database-diagram.png](docs/database-diagram.png). The schema reflects the existing Domain model; it does not add a student number, equipment type, or return timestamp that the Domain does not contain.

| Table | Columns and constraints |
| --- | --- |
| `Students` | `Id` integer primary key; required `Name` (maximum 120 characters); required `IsAllowedToBorrow`; required `MaximumActiveBorrowings` with a non-negative check constraint. |
| `Equipment` | `Id` integer primary key; required `Name` (maximum 160 characters); required `IsAvailable`, indexed for inventory filtering. |
| `Borrowings` | `Id` GUID primary key; required `StudentId` and `EquipmentId` foreign keys; required `DateBorrowed`, `ExpectedReturnDate`, and integer `Status` (`0` Active, `1` Returned). Status has a check constraint. Both foreign keys use `RESTRICT` on delete. |

Each student and equipment item can be referenced by multiple borrowing records over time. A filtered unique index on `Borrowings.EquipmentId` where `Status = 0` prevents two active borrowings for one equipment item. Additional indexes support borrowing counts by student and due-date ordering. Fluent mappings are in `EquipmentBorrowing.Infrastructure/Persistence/Configurations`.

### SQLite and Entity Framework Core

The Infrastructure project references `Microsoft.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.Sqlite`, and `Microsoft.EntityFrameworkCore.Design`, all version `10.0.12`. Desktop's `Microsoft.Extensions.DependencyInjection` reference is also `10.0.12`, matching the EF Core stack. The projects target .NET 10. SQLite stores data in a local file without a separate database server; EF Core maps Domain objects, translates LINQ to SQLite SQL, and applies versioned schema changes.

`EquipmentBorrowingDbContext` in Infrastructure exposes `Students`, `Equipment`, and `Borrowings`. Fluent configurations keep storage rules out of Domain and Application. The design-time factory uses an in-memory SQLite connection so migration scaffolding does not touch the persistent data file.

### Repository Transition and Dependency Injection

The Activity 2 implementation was:

```text
Repository Interface
    ↓
In-Memory Repository
```

The Desktop composition root now selects:

```text
Avalonia View
    ↓
ViewModel
    ↓
Application Service
    ↓
Repository Interface
    ↓
EF Core Repository
    ↓
EquipmentBorrowingDbContext
    ↓
SQLite
```

`EfStudentRepository`, `EfEquipmentRepository`, and `EfBorrowingRepository` use an injected `IDbContextFactory<EquipmentBorrowingDbContext>`. The factory creates and disposes a context for each repository operation; the repositories are singletons because they retain no context or mutable state. Desktop registers these EF repositories in place of the in-memory ones. The ViewModel only gained a service-backed available-equipment list so already borrowed items cannot be selected in the borrow form.

### Database File and Migrations

The persistent file is `%LOCALAPPDATA%/CampusEquipmentBorrowing/Data/equipmentborrowings.db` (normally `C:\Users\<user>\AppData\Local\CampusEquipmentBorrowing\Data\equipmentborrowings.db`). The directory is created automatically. Startup calls `Database.MigrateAsync()` before opening the main window; it never drops or recreates the database. Migrations are the schema source of truth and live in `EquipmentBorrowing.Infrastructure/Persistence/Migrations`.

The committed migration is `InitialCreate`. A local `dotnet-ef` tool manifest is in `.config/dotnet-tools.json`. Scaffold a later migration with:

```powershell
dotnet tool restore
dotnet tool run dotnet-ef migrations add MigrationName --project EquipmentBorrowing.Infrastructure --startup-project EquipmentBorrowing.Infrastructure --output-dir Persistence/Migrations
```

Apply it explicitly from PowerShell with:

```powershell
$dbPath = Join-Path $env:LOCALAPPDATA 'CampusEquipmentBorrowing\Data\equipmentborrowings.db'
dotnet tool run dotnet-ef database update --connection "Data Source=$dbPath;Foreign Keys=True" --project EquipmentBorrowing.Infrastructure --startup-project EquipmentBorrowing.Infrastructure
```

### LINQ Queries and Generated SQL

The EF repositories execute these application queries against SQLite:

1. Available equipment: filter `Equipment` by `IsAvailable`, ordered by name.
2. Active borrowing summaries: join `Borrowings` to `Students` and `Equipment`, filter to `BorrowingStatus.Active`, and order by expected return date.
3. Borrowing limit: count active `Borrowings` for one student with `CountAsync`.

Actual EF Core command output is documented in [docs/generated-sql.md](docs/generated-sql.md). The available-equipment and join statements were captured from Desktop startup logs; the SQLite persistence test also captures and checks all three commands using EF Core's `Database.Command` logger.

### Tracking vs No-Tracking

Queries used only to display equipment, students, borrowing summaries, or history use `AsNoTracking()` so EF does not retain entity snapshots. Each repository write uses a short-lived context, attaches the Domain object, and marks only the changed property (`IsAvailable` or borrowing `Status`) as modified before `SaveChangesAsync()`. A DbContext is never shared with a ViewModel.

### Seed Data and Persistence Demonstration

Startup applies pending migrations, then seeds only when all three domain tables are empty. Initial data is two students (Juan Dela Cruz can borrow, Maria Santos cannot), four equipment items (three available and a Raspberry Pi already borrowed), and an active borrowing connecting Juan to the Raspberry Pi. Repeated startup does not duplicate seed rows or reset existing records.

`SqlitePersistenceTests` creates an isolated SQLite file, runs initialization twice, verifies idempotent seed data, verifies an unsuccessful borrowing, borrows available equipment, and creates fresh repository/context factories to confirm the active borrowing survives a simulated close and reopen. It then returns the equipment and uses another fresh factory to verify that the returned status and available state persist. This exercises the application services and EF repositories without changing the user's database.

### Architectural Reflection

1. **Why did SQLite not require a complete rewrite?** Use cases already depended on repository interfaces, so Infrastructure and Desktop registrations could change while Domain rules, Application services, and the MVVM workflow remained in place.
2. **Why should a ViewModel not use `DbContext` directly?** That couples presentation state to EF Core, puts persistence decisions in the UI layer, and makes the ViewModel harder to test or reuse.
3. **What responsibility does the repository implementation perform?** It translates repository operations into EF Core queries and updates, manages short-lived DbContext instances, and persists Domain state through SQLite.
4. **What is the purpose of an EF Core migration?** It records an ordered, reviewable schema change that can be applied consistently to create or update databases.
5. **Why are foreign keys important?** They prevent a borrowing from referring to a missing student or equipment item and preserve the Domain relationships.
6. **Why can a read-only query benefit from `AsNoTracking()`?** EF skips change-tracking snapshots for entities that will only be displayed, reducing tracking work and accidental state coupling.
7. **What if SQLite is replaced by another provider?** Views, ViewModels, Application services, and repository interfaces can remain; Infrastructure adapts provider configuration and repository implementations.
