using Domain.Entities.BookingAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.ScheduleId)
            .IsRequired();

        builder.Property(b => b.SeatNumber)
            .IsRequired();

        builder.Property(b => b.PassengerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.PassengerEmail)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.BookingType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(b => b.BookingDate)
            .IsRequired();

        builder.HasIndex(b => b.PassengerEmail);
        builder.HasIndex(b => new { b.ScheduleId, b.SeatNumber });

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt)
            .IsRequired();
    }
}