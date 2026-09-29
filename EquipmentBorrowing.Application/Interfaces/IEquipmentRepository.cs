using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Interfaces;

public interface IEquipmentRepository
{
    Task<IEnumerable<Equipment>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Equipment?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Equipment equipment,
        CancellationToken cancellationToken = default);
}