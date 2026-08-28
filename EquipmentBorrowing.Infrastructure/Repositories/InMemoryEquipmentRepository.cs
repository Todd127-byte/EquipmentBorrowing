using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryEquipmentRepository : IEquipmentRepository
{
    private readonly List<Equipment> _equipment;

    public InMemoryEquipmentRepository()
    {
        _equipment = new List<Equipment>
        {
            new Equipment(
                101,
                "Arduino Uno",
                true),

            new Equipment(
                102,
                "Raspberry Pi",
                false)
        };
    }

    public Task<Equipment?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var equipment = _equipment
            .FirstOrDefault(e => e.Id == id);

        return Task.FromResult(equipment);
    }

    public Task UpdateAsync(
        Equipment equipment,
        CancellationToken cancellationToken = default)
    {
        var existingEquipment = _equipment
            .FirstOrDefault(e => e.Id == equipment.Id);

        if (existingEquipment is not null)
        {
            existingEquipment.MarkAsAvailable();

            if (!equipment.IsAvailable)
            {
                existingEquipment.MarkAsBorrowed();
            }
        }

        return Task.CompletedTask;
    }
}