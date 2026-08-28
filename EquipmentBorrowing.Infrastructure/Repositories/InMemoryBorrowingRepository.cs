using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryBorrowingRepository : IBorrowingRepository
{
    private readonly List<Borrowing> _borrowings;

    public InMemoryBorrowingRepository()
    {
        _borrowings = new List<Borrowing>();
    }

    public Task AddAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default)
    {
        _borrowings.Add(borrowing);

        return Task.CompletedTask;
    }

    public Task<int> CountActiveByStudentIdAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var count = _borrowings.Count(b =>
            b.StudentId == studentId &&
            b.Status == BorrowingStatus.Active);

        return Task.FromResult(count);
    }
}