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

    public Task<IEnumerable<Borrowing>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Borrowing>>(_borrowings);
    }

    public Task<Borrowing?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_borrowings.FirstOrDefault(b => b.Id == id));
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

    public Task UpdateAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default)
    {
        var index = _borrowings.FindIndex(b => b.Id == borrowing.Id);
        if (index >= 0)
        {
            _borrowings[index] = borrowing;
        }

        return Task.CompletedTask;
    }
}
