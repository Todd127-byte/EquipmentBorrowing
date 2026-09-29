using EquipmentBorrowing.Application.Models;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Interfaces;

public interface IBorrowingRepository
{
    Task<IReadOnlyList<ActiveBorrowingSummary>> GetActiveSummariesAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Borrowing>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Borrowing?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default);

    Task<int> CountActiveByStudentIdAsync(
        int studentId,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default);
}
