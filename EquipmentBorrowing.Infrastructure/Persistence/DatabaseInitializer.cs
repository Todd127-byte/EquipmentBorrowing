using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Persistence;

public sealed class DatabaseInitializer(
    IDbContextFactory<EquipmentBorrowingDbContext> contextFactory)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await context.Database.MigrateAsync(cancellationToken);

        // Seed only a completely empty database. Existing user data is never reset or overwritten.
        if (await context.Students.AnyAsync(cancellationToken) ||
            await context.Equipment.AnyAsync(cancellationToken) ||
            await context.Borrowings.AnyAsync(cancellationToken))
        {
            return;
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        context.Students.AddRange(
            new Student(1, "Juan Dela Cruz", isAllowedToBorrow: true, maximumActiveBorrowings: 3),
            new Student(2, "Maria Santos", isAllowedToBorrow: false, maximumActiveBorrowings: 3));

        context.Equipment.AddRange(
            new Equipment(101, "Arduino Uno", isAvailable: true),
            new Equipment(102, "Raspberry Pi", isAvailable: false),
            new Equipment(103, "USB Webcam", isAvailable: true),
            new Equipment(104, "Portable Projector", isAvailable: true));

        context.Borrowings.Add(new Borrowing(
            studentId: 1,
            equipmentId: 102,
            dateBorrowed: DateTime.Today.AddDays(-2),
            expectedReturnDate: DateTime.Today.AddDays(12)));

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
