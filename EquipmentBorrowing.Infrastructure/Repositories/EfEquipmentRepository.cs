using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public sealed class EfEquipmentRepository(
    IDbContextFactory<EquipmentBorrowingDbContext> contextFactory)
    : IEquipmentRepository
{
    public async Task<IEnumerable<Equipment>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Equipment
            .AsNoTracking()
            .OrderBy(equipment => equipment.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Equipment>> GetAvailableAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Equipment
            .AsNoTracking()
            .Where(equipment => equipment.IsAvailable)
            .OrderBy(equipment => equipment.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Equipment?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Equipment
            .AsNoTracking()
            .FirstOrDefaultAsync(equipment => equipment.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(
        Equipment equipment,
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        context.Attach(equipment);
        context.Entry(equipment)
            .Property(item => item.IsAvailable)
            .IsModified = true;
        await context.SaveChangesAsync(cancellationToken);
    }
}
