using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class ReturnEquipmentService(
    IBorrowingRepository borrowingRepository,
    IEquipmentRepository equipmentRepository)
{
    public async Task ReturnEquipmentAsync(
        Guid borrowingId,
        CancellationToken cancellationToken = default)
    {
        var borrowing = await borrowingRepository.GetByIdAsync(
            borrowingId,
            cancellationToken);

        if (borrowing is null)
        {
            throw new InvalidOperationException("Borrowing record was not found.");
        }

        if (borrowing.Status != BorrowingStatus.Active)
        {
            throw new InvalidOperationException("Borrowing has already been returned.");
        }

        var equipment = await equipmentRepository.GetByIdAsync(
            borrowing.EquipmentId,
            cancellationToken);

        if (equipment is null)
        {
            throw new InvalidOperationException("Equipment record was not found.");
        }

        borrowing.MarkAsReturned();
        equipment.MarkAsAvailable();

        await equipmentRepository.UpdateAsync(equipment, cancellationToken);
        await borrowingRepository.UpdateAsync(borrowing, cancellationToken);
    }
}
