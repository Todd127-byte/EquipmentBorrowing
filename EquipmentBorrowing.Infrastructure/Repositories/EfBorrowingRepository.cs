using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Models;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public sealed class EfBorrowingRepository(
    IDbContextFactory<EquipmentBorrowingDbContext> contextFactory)
    : IBorrowingRepository
{
    public async Task<IReadOnlyList<ActiveBorrowingSummary>> GetActiveSummariesAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var query =
            from borrowing in context.Borrowings.AsNoTracking()
            join student in context.Students.AsNoTracking()
                on borrowing.StudentId equals student.Id
            join equipment in context.Equipment.AsNoTracking()
                on borrowing.EquipmentId equals equipment.Id
            where borrowing.Status == BorrowingStatus.Active
            orderby borrowing.ExpectedReturnDate
            select new ActiveBorrowingSummary(
                borrowing.Id,
                student.Id,
                student.Name,
                equipment.Id,
                equipment.Name,
                borrowing.ExpectedReturnDate,
                borrowing.Status);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Borrowing>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Borrowings
            .AsNoTracking()
            .OrderByDescending(borrowing => borrowing.DateBorrowed)
            .ToListAsync(cancellationToken);
    }

    public async Task<Borrowing?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Borrowings
            .AsNoTracking()
            .FirstOrDefaultAsync(borrowing => borrowing.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await context.Borrowings.AddAsync(borrowing, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountActiveByStudentIdAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Borrowings
            .AsNoTracking()
            .CountAsync(
                borrowing => borrowing.StudentId == studentId &&
                             borrowing.Status == BorrowingStatus.Active,
                cancellationToken);
    }

    public async Task UpdateAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        context.Attach(borrowing);
        context.Entry(borrowing)
            .Property(item => item.Status)
            .IsModified = true;
        await context.SaveChangesAsync(cancellationToken);
    }
}
