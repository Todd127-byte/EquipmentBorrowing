using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Models;

public sealed record ActiveBorrowingSummary(
    Guid BorrowingId,
    int StudentId,
    string StudentName,
    int EquipmentId,
    string EquipmentName,
    DateTime ExpectedReturnDate,
    BorrowingStatus Status);
