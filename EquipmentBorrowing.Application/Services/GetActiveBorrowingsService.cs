using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Models;

namespace EquipmentBorrowing.Application.Services;

public sealed class GetActiveBorrowingsService(IBorrowingRepository borrowingRepository)
{
    public Task<IReadOnlyList<ActiveBorrowingSummary>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        return borrowingRepository.GetActiveSummariesAsync(cancellationToken);
    }
}
