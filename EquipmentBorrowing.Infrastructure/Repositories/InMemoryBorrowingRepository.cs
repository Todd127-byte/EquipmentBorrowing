using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Models;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryBorrowingRepository : IBorrowingRepository
{
    private readonly List<Borrowing> _borrowings = new();
    private readonly IStudentRepository _studentRepository;
    private readonly IEquipmentRepository _equipmentRepository;

    public InMemoryBorrowingRepository(
        IStudentRepository? studentRepository = null,
        IEquipmentRepository? equipmentRepository = null)
    {
        _studentRepository = studentRepository ?? new InMemoryStudentRepository();
        _equipmentRepository = equipmentRepository ?? new InMemoryEquipmentRepository();
    }

    public async Task<IReadOnlyList<ActiveBorrowingSummary>> GetActiveSummariesAsync(
        CancellationToken cancellationToken = default)
    {
        var summaries = new List<ActiveBorrowingSummary>();
        foreach (var borrowing in _borrowings.Where(
                     item => item.Status == BorrowingStatus.Active))
        {
            var student = await _studentRepository.GetByIdAsync(
                borrowing.StudentId,
                cancellationToken);
            var equipment = await _equipmentRepository.GetByIdAsync(
                borrowing.EquipmentId,
                cancellationToken);

            summaries.Add(new ActiveBorrowingSummary(
                borrowing.Id,
                borrowing.StudentId,
                student?.Name ?? $"Student #{borrowing.StudentId}",
                borrowing.EquipmentId,
                equipment?.Name ?? $"Equipment #{borrowing.EquipmentId}",
                borrowing.ExpectedReturnDate,
                borrowing.Status));
        }

        return summaries;
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
        return Task.FromResult(_borrowings.FirstOrDefault(borrowing => borrowing.Id == id));
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
        var count = _borrowings.Count(borrowing =>
            borrowing.StudentId == studentId &&
            borrowing.Status == BorrowingStatus.Active);

        return Task.FromResult(count);
    }

    public Task UpdateAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default)
    {
        var index = _borrowings.FindIndex(existing => existing.Id == borrowing.Id);
        if (index >= 0)
        {
            _borrowings[index] = borrowing;
        }

        return Task.CompletedTask;
    }
}
