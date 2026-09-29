namespace EquipmentBorrowing.Domain;

public class Borrowing
{
    public Guid Id { get; }
    public int StudentId { get; }
    public int EquipmentId { get; }
    public DateTime DateBorrowed { get; }
    public DateTime ExpectedReturnDate { get; }
    public BorrowingStatus Status { get; private set; }

    public Borrowing(
        int studentId,
        int equipmentId,
        DateTime dateBorrowed,
        DateTime expectedReturnDate,
        Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        StudentId = studentId;
        EquipmentId = equipmentId;
        DateBorrowed = dateBorrowed;
        ExpectedReturnDate = expectedReturnDate;
        Status = BorrowingStatus.Active;
    }

    // EF Core materializes persisted records through this private constructor.
    private Borrowing()
    {
    }

    public void MarkAsReturned()
    {
        if (Status == BorrowingStatus.Returned)
        {
            throw new InvalidOperationException("Borrowing has already been returned.");
        }

        Status = BorrowingStatus.Returned;
    }
}
