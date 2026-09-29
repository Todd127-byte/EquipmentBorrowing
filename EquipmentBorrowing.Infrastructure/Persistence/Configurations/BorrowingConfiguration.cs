using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public sealed class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
{
    public void Configure(EntityTypeBuilder<Borrowing> builder)
    {
        builder.ToTable("Borrowings", table =>
            table.HasCheckConstraint(
                "CK_Borrowings_ValidStatus",
                "\"Status\" IN (0, 1)"));

        builder.HasKey(borrowing => borrowing.Id);
        builder.Property(borrowing => borrowing.Id).ValueGeneratedNever();
        builder.Property(borrowing => borrowing.StudentId).IsRequired();
        builder.Property(borrowing => borrowing.EquipmentId).IsRequired();
        builder.Property(borrowing => borrowing.DateBorrowed).IsRequired();
        builder.Property(borrowing => borrowing.ExpectedReturnDate).IsRequired();
        builder.Property(borrowing => borrowing.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne<Student>()
            .WithMany()
            .HasForeignKey(borrowing => borrowing.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Equipment>()
            .WithMany()
            .HasForeignKey(borrowing => borrowing.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(borrowing => borrowing.StudentId);
        builder.HasIndex(borrowing => new { borrowing.StudentId, borrowing.Status });
        builder.HasIndex(borrowing => borrowing.ExpectedReturnDate);
        builder.HasIndex(borrowing => borrowing.EquipmentId)
            .IsUnique()
            .HasFilter("\"Status\" = 0");
    }
}
