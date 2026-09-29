using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Interfaces;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Student?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}
