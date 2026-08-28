namespace EquipmentBorrowing.Domain;

public class Borrowing
{
    public int StudentId { get; }
    public int EquipmentId { get; }
    public DateTime DateBorrowed { get; }
    public DateTime ExpectedReturnDate { get; }
    public BorrowingStatus Status { get; private set; }

    public Borrowing(
        int studentId,
        int equipmentId,
        DateTime dateBorrowed,
        DateTime expectedReturnDate)
    {
        StudentId = studentId;
        EquipmentId = equipmentId;
        DateBorrowed = dateBorrowed;
        ExpectedReturnDate = expectedReturnDate;
        Status = BorrowingStatus.Active;
    }

    public void MarkAsReturned()
    {
        Status = BorrowingStatus.Returned;
    }
}