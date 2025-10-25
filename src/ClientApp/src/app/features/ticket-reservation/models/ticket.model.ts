export interface SearchParams {
  from: string;
  to: string;
  journeyDate: Date | null;
}

export interface TrendingRoute {
  from: string;
  to: string;
  popular: boolean;
}

export interface Bus {
  id: string;
  name: string;
  from: string;
  to: string;
  departureTime: string;
  arrivalTime: string;
  fare: number;
  seatsLeft: number;
  isAC: boolean;
  totalSeats: number;
  bookedSeats: number[];
}

export interface Seat {
  seatNumber: number;
  isBooked: boolean;
  isSelected: boolean;
}

export interface BookingDetails {
  busId: string;
  seatNumbers: number[];
  passengerName: string;
  mobileNumber: string;
}