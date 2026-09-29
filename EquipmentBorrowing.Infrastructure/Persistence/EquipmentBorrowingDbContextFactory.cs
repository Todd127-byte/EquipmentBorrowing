using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EquipmentBorrowing.Infrastructure.Persistence;

public sealed class EquipmentBorrowingDbContextFactory
    : IDesignTimeDbContextFactory<EquipmentBorrowingDbContext>
{
    public EquipmentBorrowingDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<EquipmentBorrowingDbContext>()
            // Migration scaffolding only needs the model. Keep design-time commands
            // independent from the user's persistent database file.
            .UseSqlite("Data Source=:memory:;Foreign Keys=True")
            .Options;

        return new EquipmentBorrowingDbContext(options);
    }
}
