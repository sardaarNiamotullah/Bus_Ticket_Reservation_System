using Domain.Entities.BusAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class BusConfiguration : IEntityTypeConfiguration<Bus>
{
    public void Configure(EntityTypeBuilder<Bus> builder)
    {
        builder.ToTable("Buses");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.BusNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(b => b.BusNumber)
            .IsUnique();

        builder.Property(b => b.IsAC)
            .IsRequired();

        builder.Property(b => b.TotalSeats)
            .IsRequired();

        // Owned type for Money value object
        builder.OwnsOne(b => b.FarePerSeat, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("FareAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("BDT");
        });

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt)
            .IsRequired();
    }
}