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