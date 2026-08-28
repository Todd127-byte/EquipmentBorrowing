using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Infrastructure.Repositories;

Console.WriteLine("=== Campus Equipment Borrowing System ===");
Console.WriteLine();

// Create repositories
var studentRepository = new InMemoryStudentRepository();
var equipmentRepository = new InMemoryEquipmentRepository();
var borrowingRepository = new InMemoryBorrowingRepository();

// Create application service
var borrowingService = new BorrowEquipmentService(
    studentRepository,
    equipmentRepository,
    borrowingRepository);

// ----------------------------------------
// SUCCESSFUL CASE
// ----------------------------------------

Console.WriteLine("SUCCESSFUL CASE");
Console.WriteLine("----------------");

try
{
    await borrowingService.BorrowEquipmentAsync(
        studentId: 1,
        equipmentId: 101,
        expectedReturnDate: DateTime.Now.AddDays(7));

    Console.WriteLine("Borrowing successful!");
}
catch (Exception ex)
{
    Console.WriteLine($"Borrowing failed: {ex.Message}");
}

Console.WriteLine();

// ----------------------------------------
// FAILURE CASE
// ----------------------------------------

Console.WriteLine("FAILURE CASE");
Console.WriteLine("------------");

try
{
    await borrowingService.BorrowEquipmentAsync(
        studentId: 1,
        equipmentId: 102,
        expectedReturnDate: DateTime.Now.AddDays(7));

    Console.WriteLine("Borrowing successful!");
}
catch (Exception ex)
{
    Console.WriteLine($"Borrowing failed: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();