using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<Student> _students;

    public InMemoryStudentRepository()
    {
        _students = new List<Student>
        {
            new Student(
                1,
                "Juan Dela Cruz",
                true,
                3),

            new Student(
                2,
                "Maria Santos",
                false,
                3)
        };
    }

    public Task<Student?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var student = _students
            .FirstOrDefault(s => s.Id == id);

        return Task.FromResult(student);
    }
}