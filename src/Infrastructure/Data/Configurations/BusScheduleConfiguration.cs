using Domain.Entities.ScheduleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class BusScheduleConfiguration : IEntityTypeConfiguration<BusSchedule>
{
    public void Configure(EntityTypeBuilder<BusSchedule> builder)
    {
        builder.ToTable("BusSchedules");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.BusId)
            .IsRequired();

        builder.Property(s => s.RouteId)
            .IsRequired();

        builder.Property(s => s.JourneyDate)
            .IsRequired();

        builder.Property(s => s.AvailableSeats)
            .IsRequired();

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // One-to-many relationship with Seats
        builder.HasMany(s => s.Seats)
            .WithOne()
            .HasForeignKey(seat => seat.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.BusId, s.RouteId, s.JourneyDate })
            .IsUnique();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();
    }
}