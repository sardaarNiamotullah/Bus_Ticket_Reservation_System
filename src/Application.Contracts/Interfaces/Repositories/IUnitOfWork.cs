namespace Application.Contracts.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IBusRepository Buses { get; }
    IRouteRepository Routes { get; }
    IBusScheduleRepository BusSchedules { get; }
    IBookingRepository Bookings { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
