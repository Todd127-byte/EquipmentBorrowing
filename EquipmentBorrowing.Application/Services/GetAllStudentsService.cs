using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class GetAllStudentsService(IStudentRepository studentRepository)
{
    public Task<IEnumerable<Student>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        return studentRepository.GetAllAsync(cancellationToken);
    }
}
