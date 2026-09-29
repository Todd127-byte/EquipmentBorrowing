# EF Core Generated SQL Evidence

These statements were captured from the Debug `Microsoft.EntityFrameworkCore.Database.Command` output while the Avalonia Desktop application started and loaded its equipment and active borrowing lists on 2026-09-29. They are the SQL emitted by the EF Core 10.0.12 SQLite provider for the current LINQ queries. Values are parameterized where a query has runtime inputs.

## Available equipment

LINQ source: `EfEquipmentRepository.GetAvailableAsync`. The view calls this query when it refreshes the borrow form. It returns equipment whose `IsAvailable` flag is true, ordered by name.

```sql
SELECT "e"."Id", "e"."IsAvailable", "e"."Name"
FROM "Equipment" AS "e"
WHERE "e"."IsAvailable"
ORDER BY "e"."Name"
```

## Active borrowings with student and equipment

LINQ source: `EfBorrowingRepository.GetActiveSummariesAsync`. The two joins supply the student and equipment names displayed by the Active Borrowings view. The status value `0` represents `BorrowingStatus.Active`.

```sql
SELECT "b"."Id", "s"."Id", "s"."Name", "e"."Id", "e"."Name", "b"."ExpectedReturnDate", "b"."Status"
FROM "Borrowings" AS "b"
INNER JOIN "Students" AS "s" ON "b"."StudentId" = "s"."Id"
INNER JOIN "Equipment" AS "e" ON "b"."EquipmentId" = "e"."Id"
WHERE "b"."Status" = 0
ORDER BY "b"."ExpectedReturnDate"
```

## Active borrowing count for one student

LINQ source: `EfBorrowingRepository.CountActiveByStudentIdAsync`. This `CountAsync` query runs as part of the borrowing limit check. The student ID is passed as a parameter; the active status is stored as the enum's integer value.

```sql
SELECT COUNT(*)
FROM "Borrowings" AS "b"
WHERE "b"."StudentId" = @studentId AND "b"."Status" = 0
```

The first two statements above are verbatim from the captured application command log. The count statement was captured through the same EF Core command logger in the SQLite persistence test, which also checks its parameterized predicate.
