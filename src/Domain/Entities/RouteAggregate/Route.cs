namespace Domain.Entities.RouteAggregate;

public class Route : BaseEntity
{
    public string FromCity { get; private set; }
    public string ToCity { get; private set; }
    public TimeSpan DepartureTime { get; private set; }
    public TimeSpan ArrivalTime { get; private set; }
    public int DurationMinutes { get; private set; }

    private Route() {     
    FromCity = null!;
    ToCity = null!;
    } // For EF Core

    public Route(string fromCity, string toCity, TimeSpan departureTime, TimeSpan arrivalTime)
    {
        if (string.IsNullOrWhiteSpace(fromCity))
            throw new ArgumentException("From city cannot be empty", nameof(fromCity));

        if (string.IsNullOrWhiteSpace(toCity))
            throw new ArgumentException("To city cannot be empty", nameof(toCity));

        if (fromCity.Equals(toCity, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("From and To cities cannot be the same");

        if (arrivalTime <= departureTime)
            throw new ArgumentException("Arrival time must be after departure time");

        FromCity = fromCity;
        ToCity = toCity;
        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
        DurationMinutes = CalculateDuration(departureTime, arrivalTime);
    }

    private int CalculateDuration(TimeSpan departure, TimeSpan arrival)
    {
        var duration = arrival - departure;
        if (duration.TotalMinutes < 0)
            duration = duration.Add(TimeSpan.FromDays(1)); // Next day arrival

        return (int)duration.TotalMinutes;
    }

    public void UpdateTiming(TimeSpan departureTime, TimeSpan arrivalTime)
    {
        if (arrivalTime <= departureTime)
            throw new ArgumentException("Arrival time must be after departure time");

        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
        DurationMinutes = CalculateDuration(departureTime, arrivalTime);
        UpdateTimestamp();
    }
}