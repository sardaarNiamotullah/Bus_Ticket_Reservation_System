using Application.Contracts.Interfaces.Services;
using Application.Mappings;
using Application.Services;
using Domain.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        // Register Domain Services
        services.AddScoped<ISeatBookingDomainService, SeatBookingDomainService>();

        // Register Application Services
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}