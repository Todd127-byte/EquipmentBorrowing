using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class BorrowEquipmentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IBorrowingRepository _borrowingRepository;

    public BorrowEquipmentService(
        IStudentRepository studentRepository,
        IEquipmentRepository equipmentRepository,
        IBorrowingRepository borrowingRepository)
    {
        _studentRepository = studentRepository;
        _equipmentRepository = equipmentRepository;
        _borrowingRepository = borrowingRepository;
    }

    public async Task BorrowEquipmentAsync(
        int studentId,
        int equipmentId,
        DateTime expectedReturnDate,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(
            studentId,
            cancellationToken);

        if (student is null)
        {
            throw new InvalidOperationException(
                "Student does not exist.");
        }

        if (!student.IsAllowedToBorrow)
        {
            throw new InvalidOperationException(
                "Student is not allowed to borrow equipment.");
        }

        var equipment = await _equipmentRepository.GetByIdAsync(
            equipmentId,
            cancellationToken);

        if (equipment is null)
        {
            throw new InvalidOperationException(
                "Equipment does not exist.");
        }

        if (!equipment.IsAvailable)
        {
            throw new InvalidOperationException(
                "Equipment is not available.");
        }

        var activeBorrowings =
            await _borrowingRepository.CountActiveByStudentIdAsync(
                studentId,
                cancellationToken);

        if (activeBorrowings >= student.MaximumActiveBorrowings)
        {
            throw new InvalidOperationException(
                "Student has reached the maximum number of active borrowings.");
        }

        var borrowing = new Borrowing(
            student.Id,
            equipment.Id,
            DateTime.Now,
            expectedReturnDate);

        equipment.MarkAsBorrowed();

        await _equipmentRepository.UpdateAsync(
            equipment,
            cancellationToken);

        await _borrowingRepository.AddAsync(
            borrowing,
            cancellationToken);
    }
}