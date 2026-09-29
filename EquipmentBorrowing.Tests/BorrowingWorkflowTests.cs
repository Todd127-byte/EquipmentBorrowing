using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Repositories;

namespace EquipmentBorrowing.Tests;

public class BorrowingWorkflowTests
{
    [Fact]
    public async Task BorrowEquipment_marks_equipment_unavailable_and_creates_active_borrowing()
    {
        var studentRepository = new InMemoryStudentRepository();
        var equipmentRepository = new InMemoryEquipmentRepository();
        var borrowingRepository = new InMemoryBorrowingRepository();
        var service = new BorrowEquipmentService(
            studentRepository,
            equipmentRepository,
            borrowingRepository);

        await service.BorrowEquipmentAsync(1, 101, FutureDate());

        var equipment = await equipmentRepository.GetByIdAsync(101);
        Assert.NotNull(equipment);
        Assert.False(equipment.IsAvailable);
        Assert.Equal(1, await borrowingRepository.CountActiveByStudentIdAsync(1));
    }

    [Fact]
    public async Task BorrowEquipment_rejects_student_not_allowed_to_borrow()
    {
        var equipmentRepository = new InMemoryEquipmentRepository();
        var service = new BorrowEquipmentService(
            new InMemoryStudentRepository(),
            equipmentRepository,
            new InMemoryBorrowingRepository());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.BorrowEquipmentAsync(2, 101, FutureDate()));

        Assert.Contains("not allowed", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.True((await equipmentRepository.GetByIdAsync(101))!.IsAvailable);
    }

    [Fact]
    public async Task BorrowEquipment_rejects_unavailable_equipment()
    {
        var service = new BorrowEquipmentService(
            new InMemoryStudentRepository(),
            new InMemoryEquipmentRepository(),
            new InMemoryBorrowingRepository());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.BorrowEquipmentAsync(1, 102, FutureDate()));

        Assert.Contains("not available", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task BorrowEquipment_rejects_student_at_active_borrowing_limit()
    {
        var borrowingRepository = new InMemoryBorrowingRepository();
        for (var i = 0; i < 3; i++)
        {
            await borrowingRepository.AddAsync(new Borrowing(1, 900 + i, DateTime.Today, FutureDate()));
        }

        var equipmentRepository = new InMemoryEquipmentRepository();
        var service = new BorrowEquipmentService(
            new InMemoryStudentRepository(),
            equipmentRepository,
            borrowingRepository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.BorrowEquipmentAsync(1, 101, FutureDate()));

        Assert.Contains("maximum", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.True((await equipmentRepository.GetByIdAsync(101))!.IsAvailable);
    }

    [Fact]
    public async Task BorrowEquipment_rejects_expected_return_date_that_is_not_in_the_future()
    {
        var service = new BorrowEquipmentService(
            new InMemoryStudentRepository(),
            new InMemoryEquipmentRepository(),
            new InMemoryBorrowingRepository());

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.BorrowEquipmentAsync(1, 101, DateTime.Today));
    }

    [Fact]
    public async Task ReturnEquipment_marks_borrowing_returned_and_equipment_available()
    {
        var studentRepository = new InMemoryStudentRepository();
        var equipmentRepository = new InMemoryEquipmentRepository();
        var borrowingRepository = new InMemoryBorrowingRepository();
        var borrowService = new BorrowEquipmentService(
            studentRepository,
            equipmentRepository,
            borrowingRepository);
        await borrowService.BorrowEquipmentAsync(1, 101, FutureDate());

        var borrowing = Assert.Single(await borrowingRepository.GetAllAsync());
        var returnService = new ReturnEquipmentService(borrowingRepository, equipmentRepository);
        await returnService.ReturnEquipmentAsync(borrowing.Id);

        Assert.Equal(BorrowingStatus.Returned, borrowing.Status);
        Assert.True((await equipmentRepository.GetByIdAsync(101))!.IsAvailable);
        Assert.Equal(0, await borrowingRepository.CountActiveByStudentIdAsync(1));
    }

    [Fact]
    public async Task ReturnEquipment_rejects_a_borrowing_that_was_already_returned()
    {
        var equipmentRepository = new InMemoryEquipmentRepository();
        var borrowingRepository = new InMemoryBorrowingRepository();
        var borrowing = new Borrowing(1, 101, DateTime.Today, FutureDate());
        borrowing.MarkAsReturned();
        await borrowingRepository.AddAsync(borrowing);
        var service = new ReturnEquipmentService(borrowingRepository, equipmentRepository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ReturnEquipmentAsync(borrowing.Id));

        Assert.Contains("already been returned", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ReturnEquipment_reports_a_missing_borrowing_record()
    {
        var service = new ReturnEquipmentService(
            new InMemoryBorrowingRepository(),
            new InMemoryEquipmentRepository());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ReturnEquipmentAsync(Guid.NewGuid()));

        Assert.Contains("not found", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static DateTime FutureDate() => DateTime.Today.AddDays(7);
}
