-- 1. Retrieve all equipment.
SELECT "Id", "Name", "IsAvailable"
FROM "Equipment"
ORDER BY "Id";

-- 2. Retrieve currently available equipment.
SELECT "Id", "Name", "IsAvailable"
FROM "Equipment"
WHERE "IsAvailable" = 1
ORDER BY "Name";

-- 3. Show each active borrowing with its student and equipment details.
SELECT
    s."Name" AS "Student",
    e."Name" AS "Equipment",
    b."DateBorrowed" AS "Borrowed",
    b."ExpectedReturnDate" AS "Due"
FROM "Borrowings" AS b
INNER JOIN "Students" AS s ON s."Id" = b."StudentId"
INNER JOIN "Equipment" AS e ON e."Id" = b."EquipmentId"
WHERE b."Status" = 0
ORDER BY b."ExpectedReturnDate";

-- 4. Count active borrowings for each student, including students with none.
SELECT
    s."Id",
    s."Name",
    COUNT(b."Id") AS "ActiveBorrowingCount"
FROM "Students" AS s
LEFT JOIN "Borrowings" AS b
    ON b."StudentId" = s."Id" AND b."Status" = 0
GROUP BY s."Id", s."Name"
ORDER BY s."Name";

-- 5. Example equipment state update after a return.
-- The application performs this change through EfEquipmentRepository.
UPDATE "Equipment"
SET "IsAvailable" = 1
WHERE "Id" = 101;
