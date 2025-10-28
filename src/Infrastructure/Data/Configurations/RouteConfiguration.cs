using Domain.Entities.RouteAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.ToTable("Routes");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.FromCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.ToCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.DepartureTime)
            .IsRequired();

        builder.Property(r => r.ArrivalTime)
            .IsRequired();

        builder.Property(r => r.DurationMinutes)
            .IsRequired();

        builder.HasIndex(r => new { r.FromCity, r.ToCity });

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .IsRequired();
    }
}