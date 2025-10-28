using Application.Contracts.DTOs.Bus;
using Application.Contracts.DTOs.Route;
using Application.Contracts.DTOs.Schedule;
using Application.Contracts.DTOs.Booking;
using Application.Contracts.DTOs.Search;
using Domain.Entities.BusAggregate;
using Domain.Entities.RouteAggregate;
using Domain.Entities.ScheduleAggregate;
using Domain.Entities.BookingAggregate;
using AutoMapper;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Bus mappings
        CreateMap<Bus, BusDto>()
            .ForMember(dest => dest.FarePerSeat, opt => opt.MapFrom(src => src.FarePerSeat.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.FarePerSeat.Currency));

        // Route mappings
        CreateMap<Route, RouteDto>();

        // Schedule mappings
        CreateMap<BusSchedule, BusScheduleDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // Seat mappings
        CreateMap<Seat, SeatDto>()
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.SeatNumber))
            .ForMember(dest => dest.Row, opt => opt.MapFrom(src => CalculateRow(src.SeatNumber)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.IsBooked, opt => opt.MapFrom(src => src.Status == Domain.Enums.SeatStatus.Booked))
            .ForMember(dest => dest.IsSold, opt => opt.MapFrom(src => src.Status == Domain.Enums.SeatStatus.Sold));

        // Booking mappings
        CreateMap<Booking, BookingResultDto>()
            .ForMember(dest => dest.BookingIds, opt => opt.Ignore())
            .ForMember(dest => dest.SeatNumbers, opt => opt.MapFrom(src => new List<int> { src.SeatNumber }))
            .ForMember(dest => dest.BookingType, opt => opt.MapFrom(src => src.BookingType.ToString()))
            .ForMember(dest => dest.Message, opt => opt.Ignore())
            .ForMember(dest => dest.Success, opt => opt.Ignore());
    }

    private static int CalculateRow(int seatNumber)
    {
        // Assuming 4 seats per row (standard bus configuration)
        return (int)Math.Ceiling(seatNumber / 4.0);
    }
}