using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students", table =>
            table.HasCheckConstraint(
                "CK_Students_MaximumActiveBorrowings_NonNegative",
                "\"MaximumActiveBorrowings\" >= 0"));

        builder.HasKey(student => student.Id);
        builder.Property(student => student.Id).ValueGeneratedNever();
        builder.Property(student => student.Name).IsRequired().HasMaxLength(120);
        builder.Property(student => student.IsAllowedToBorrow).IsRequired();
        builder.Property(student => student.MaximumActiveBorrowings).IsRequired();
    }
}
