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
