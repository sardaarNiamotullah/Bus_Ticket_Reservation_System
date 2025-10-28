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
  id: string; // This is the scheduleId from backend
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

export interface BookingDetails {
  scheduleId: string;
  seatNumbers: number[];
  passengerName: string;
  passengerEmail: string;
  bookingType: 'Book' | 'Buy';
}

export interface Seat {
  number: number;
  isBooked: boolean;
  isSold: boolean;
  isSelected: boolean;
}

export interface BookingForm {
  name: string;
  email: string;
  selectedSeats: number[];
}

export interface BookingResponse {
  bookingId: string;
  scheduleId: string;
  seatNumbers: number[];
  totalAmount: number;
  status: string;
  message: string;
}