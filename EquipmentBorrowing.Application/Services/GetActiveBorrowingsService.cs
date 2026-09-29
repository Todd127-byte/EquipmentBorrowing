using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Models;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class GetActiveBorrowingsService(
    IBorrowingRepository borrowingRepository,
    IStudentRepository studentRepository,
    IEquipmentRepository equipmentRepository)
{
    public async Task<IReadOnlyList<ActiveBorrowingSummary>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var borrowings = await borrowingRepository.GetAllAsync(cancellationToken);
        var activeBorrowings = borrowings
            .Where(borrowing => borrowing.Status == BorrowingStatus.Active)
            .ToList();

        var summaries = new List<ActiveBorrowingSummary>(activeBorrowings.Count);
        foreach (var borrowing in activeBorrowings)
        {
            var student = await studentRepository.GetByIdAsync(
                borrowing.StudentId,
                cancellationToken);
            var equipment = await equipmentRepository.GetByIdAsync(
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
}
