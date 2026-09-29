using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class GetAvailableEquipmentService(IEquipmentRepository equipmentRepository)
{
    public Task<IEnumerable<Equipment>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        return equipmentRepository.GetAvailableAsync(cancellationToken);
    }
}
