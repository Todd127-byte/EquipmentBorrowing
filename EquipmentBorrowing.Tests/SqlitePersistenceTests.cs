using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using EquipmentBorrowing.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EquipmentBorrowing.Tests;

public sealed class SqlitePersistenceTests
{
    [Fact]
    public async Task Borrowing_and_return_survive_new_context_factories_and_seed_is_idempotent()
    {
        using var database = new TemporarySqliteDatabase();
        var firstFactory = database.CreateFactory();
        var initializer = new DatabaseInitializer(firstFactory);

        await initializer.InitializeAsync();
        await initializer.InitializeAsync();

        var students = new EfStudentRepository(firstFactory);
        var equipment = new EfEquipmentRepository(firstFactory);
        var borrowings = new EfBorrowingRepository(firstFactory);

        Assert.Equal(2, (await students.GetAllAsync()).Count());
        Assert.Equal(4, (await equipment.GetAllAsync()).Count());
        Assert.Single(await borrowings.GetAllAsync());
        Assert.Equal(3, (await equipment.GetAvailableAsync()).Count());

        var sqlLog = new StringWriter();
        var queryFactory = database.CreateFactory(sqlLog);
        await new EfEquipmentRepository(queryFactory).GetAvailableAsync();
        await new EfBorrowingRepository(queryFactory).GetActiveSummariesAsync();
        await new EfBorrowingRepository(queryFactory).CountActiveByStudentIdAsync(1);
        var generatedSql = sqlLog.ToString();
        Assert.Contains("FROM \"Equipment\" AS \"e\"", generatedSql);
        Assert.Contains("INNER JOIN \"Students\" AS \"s\"", generatedSql);
        Assert.Contains("SELECT COUNT(*)", generatedSql);
        Assert.Contains("\"StudentId\" = @studentId", generatedSql);

        var borrowService = new BorrowEquipmentService(students, equipment, borrowings);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => borrowService.BorrowEquipmentAsync(2, 103, FutureDate()));

        await borrowService.BorrowEquipmentAsync(1, 101, FutureDate());
        var newBorrowing = Assert.Single(
            await borrowings.GetAllAsync(), item => item.EquipmentId == 101);
        Assert.Equal(2, await borrowings.CountActiveByStudentIdAsync(1));

        // New repository and context factory instances model a closed/reopened application.
        var reopenedFactory = database.CreateFactory();
        var reopenedBorrowings = new EfBorrowingRepository(reopenedFactory);
        var reopenedEquipment = new EfEquipmentRepository(reopenedFactory);

        var persistedSummary = Assert.Single(
            await reopenedBorrowings.GetActiveSummariesAsync(),
            item => item.EquipmentId == 101);
        Assert.Equal("Juan Dela Cruz", persistedSummary.StudentName);
        Assert.Equal("Arduino Uno", persistedSummary.EquipmentName);
        Assert.False((await reopenedEquipment.GetByIdAsync(101))!.IsAvailable);

        await new ReturnEquipmentService(reopenedBorrowings, reopenedEquipment)
            .ReturnEquipmentAsync(persistedSummary.BorrowingId);

        // A second reopen confirms both the returned status and equipment state persisted.
        var finalFactory = database.CreateFactory();
        var finalBorrowings = new EfBorrowingRepository(finalFactory);
        var finalEquipment = new EfEquipmentRepository(finalFactory);
        Assert.Equal(BorrowingStatus.Returned,
            (await finalBorrowings.GetByIdAsync(newBorrowing.Id))!.Status);
        Assert.True((await finalEquipment.GetByIdAsync(101))!.IsAvailable);
        Assert.Equal(1, await finalBorrowings.CountActiveByStudentIdAsync(1));
    }

    private static DateTime FutureDate() => DateTime.Today.AddDays(7);

    private sealed class TemporarySqliteDatabase : IDisposable
    {
        private readonly string _directory = Path.Combine(
            Path.GetTempPath(),
            "CampusEquipmentBorrowing.Tests",
            Guid.NewGuid().ToString("N"));

        public IDbContextFactory<EquipmentBorrowingDbContext> CreateFactory(
            TextWriter? commandOutput = null)
        {
            Directory.CreateDirectory(_directory);
            var databasePath = Path.Combine(_directory, "test.db");
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = databasePath,
                ForeignKeys = true,
                Pooling = false
            }.ToString();

            var optionsBuilder = new DbContextOptionsBuilder<EquipmentBorrowingDbContext>()
                .UseSqlite(connectionString);
            if (commandOutput is not null)
            {
                optionsBuilder.LogTo(
                    commandOutput.WriteLine,
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information);
            }

            var options = optionsBuilder.Options;

            return new TestDbContextFactory(options);
        }

        public void Dispose()
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, recursive: true);
            }
        }
    }

    private sealed class TestDbContextFactory(
        DbContextOptions<EquipmentBorrowingDbContext> options)
        : IDbContextFactory<EquipmentBorrowingDbContext>
    {
        public EquipmentBorrowingDbContext CreateDbContext() =>
            new(options);

        public Task<EquipmentBorrowingDbContext> CreateDbContextAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(CreateDbContext());
        }
    }
}
