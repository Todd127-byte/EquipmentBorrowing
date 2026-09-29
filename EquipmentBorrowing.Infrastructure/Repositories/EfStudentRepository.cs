using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public sealed class EfStudentRepository(
    IDbContextFactory<EquipmentBorrowingDbContext> contextFactory)
    : IStudentRepository
{
    public async Task<IEnumerable<Student>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Students
            .AsNoTracking()
            .OrderBy(student => student.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Student?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(student => student.Id == id, cancellationToken);
    }
}
