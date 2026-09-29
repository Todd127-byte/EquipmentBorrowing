using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public sealed class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipment");

        builder.HasKey(equipment => equipment.Id);
        builder.Property(equipment => equipment.Id).ValueGeneratedNever();
        builder.Property(equipment => equipment.Name).IsRequired().HasMaxLength(160);
        builder.Property(equipment => equipment.IsAvailable).IsRequired();
        builder.HasIndex(equipment => equipment.IsAvailable);
    }
}
