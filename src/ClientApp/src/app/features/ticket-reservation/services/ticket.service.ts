import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import {
  Bus,
  SearchParams,
  TrendingRoute,
  BookingDetails,
  Seat,
} from '../models/ticket.model';

// Backend API response interfaces
interface BackendBus {
  id: string;
  name: string;
  routeName: string;
  departureTime: string;
  arrivalTime: string;
  fare: number;
  availableSeats: number;
  totalSeats: number;
  isAC: boolean;
}

interface BackendSeatPlan {
  scheduleId: string;
  totalSeats: number;
  seats: Array<{
    seatNumber: number;
    status: string; // "Available", "Booked", "Sold"
  }>;
}

interface BackendBookingRequest {
  scheduleId: string;
  seatNumbers: number[];
  passengerName: string;
  passengerEmail: string;
  bookingType: string; // "Book" or "Buy"
}

interface BackendBookingResponse {
  bookingId: string;
  scheduleId: string;
  seatNumbers: number[];
  totalAmount: number;
  status: string;
  message: string;
}

@Injectable({
  providedIn: 'root',
})
export class TicketService {
  private apiUrl = 'http://localhost:5123/api'; // Your .NET backend URL

  // BehaviorSubject holds the current state and emits to subscribers
  private searchParamsSubject = new BehaviorSubject<SearchParams | null>(null);
  public searchParams$: Observable<SearchParams | null> =
    this.searchParamsSubject.asObservable();

  private selectedBusSubject = new BehaviorSubject<Bus | null>(null);
  public selectedBus$: Observable<Bus | null> =
    this.selectedBusSubject.asObservable();

  constructor(private http: HttpClient) {}

  // Mock data for trending routes (keep as is)
  getTrendingRoutes(): TrendingRoute[] {
    return [
      { from: 'Dhaka', to: 'Chittagong', popular: true },
      { from: 'Dhaka', to: 'Sylhet', popular: true },
      { from: 'Dhaka', to: 'Rajshahi', popular: true },
      { from: 'Dhaka', to: 'Khulna', popular: true },
    ];
  }

  // Update search parameters
  setSearchParams(params: SearchParams): void {
    this.searchParamsSubject.next(params);
  }

  // Get current search params
  getSearchParams(): SearchParams | null {
    return this.searchParamsSubject.value;
  }

  // Search buses from backend API
  searchBuses(params: SearchParams): Observable<Bus[]> {
    const searchRequest = {
      from: params.from,
      to: params.to,
      journeyDate:
        params.journeyDate?.toISOString() || new Date().toISOString(),
    };

    return this.http
      .post<BackendBus[]>(`${this.apiUrl}/Search/buses`, searchRequest)
      .pipe(
        map((backendBuses) =>
          this.mapBackendBusesToFrontend(backendBuses, params)
        ),
        catchError(this.handleError)
      );
  }

  // Get seat plan for a specific bus
  getSeatPlan(scheduleId: string): Observable<Seat[]> {
    return this.http
      .get<BackendSeatPlan>(`${this.apiUrl}/Booking/seat-plan/${scheduleId}`)
      .pipe(
        map((response) => this.mapBackendSeatsToFrontend(response.seats)),
        catchError(this.handleError)
      );
  }

  // Book or buy seats
  bookSeats(bookingDetails: {
    scheduleId: string;
    seatNumbers: number[];
    passengerName: string;
    passengerEmail: string;
    bookingType: 'Book' | 'Buy';
  }): Observable<BackendBookingResponse> {
    const request: BackendBookingRequest = {
      scheduleId: bookingDetails.scheduleId,
      seatNumbers: bookingDetails.seatNumbers,
      passengerName: bookingDetails.passengerName,
      passengerEmail: bookingDetails.passengerEmail,
      bookingType: bookingDetails.bookingType,
    };

    return this.http
      .post<BackendBookingResponse>(`${this.apiUrl}/Booking/book`, request)
      .pipe(
        tap((response) => console.log('Booking response:', response)),
        catchError(this.handleError)
      );
  }

  // Set selected bus for booking
  setSelectedBus(bus: Bus): void {
    this.selectedBusSubject.next(bus);
  }

  // Get selected bus
  getSelectedBus(): Bus | null {
    return this.selectedBusSubject.value;
  }

  // Map backend bus data to frontend format
  private mapBackendBusesToFrontend(
    backendBuses: BackendBus[],
    params: SearchParams
  ): Bus[] {
    return backendBuses.map((bus) => ({
      id: bus.id, // This is the scheduleId
      name: bus.name,
      from: params.from,
      to: params.to,
      departureTime: this.formatTime(bus.departureTime),
      arrivalTime: this.formatTime(bus.arrivalTime),
      fare: bus.fare,
      seatsLeft: bus.availableSeats,
      isAC: bus.isAC,
      totalSeats: bus.totalSeats,
      bookedSeats: [], // Will be populated when seat plan is fetched
    }));
  }

  // Map backend seats to frontend format
  private mapBackendSeatsToFrontend(
    backendSeats: Array<{ seatNumber: number; status: string }>
  ): Seat[] {
    return backendSeats.map((seat) => ({
      number: seat.seatNumber,
      isBooked: seat.status === 'Booked',
      isSold: seat.status === 'Sold',
      isSelected: false,
    }));
  }

  // Format time from ISO string to readable format
  // private formatTime(isoTime: string): string {
  //   const date = new Date(isoTime);
  //   let hours = date.getHours();
  //   const minutes = date.getMinutes();
  //   const ampm = hours >= 12 ? 'PM' : 'AM';
  //   hours = hours % 12;
  //   hours = hours ? hours : 12; // 0 should be 12
  //   const minutesStr = minutes < 10 ? '0' + minutes : minutes.toString();
  //   return `${hours}:${minutesStr} ${ampm}`;
  // }

  private formatTime(timeString: string): string {
    // If it's already in HH:MM:SS format, return as is
    if (timeString.match(/^\d{1,2}:\d{2}:\d{2}$/)) {
      const [hours, minutes] = timeString.split(':');
      const hourNum = parseInt(hours, 10);
      const ampm = hourNum >= 12 ? 'PM' : 'AM';
      const displayHours = hourNum % 12 || 12;
      return `${displayHours}:${minutes} ${ampm}`;
    }

    // If it's ISO string, parse it
    const date = new Date(timeString);
    let hours = date.getHours();
    const minutes = date.getMinutes();
    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12;
    const minutesStr = minutes < 10 ? '0' + minutes : minutes.toString();
    return `${hours}:${minutesStr} ${ampm}`;
  }

  calculateDuration(departureTime: string, arrivalTime: string): string {
    try {
      let depHours: number, depMinutes: number;
      let arrHours: number, arrMinutes: number;

      // Parse departure time
      if (departureTime.match(/^\d{1,2}:\d{2}:\d{2}$/)) {
        [depHours, depMinutes] = departureTime.split(':').map(Number);
      } else {
        const depDate = new Date(departureTime);
        depHours = depDate.getHours();
        depMinutes = depDate.getMinutes();
      }

      // Parse arrival time
      if (arrivalTime.match(/^\d{1,2}:\d{2}:\d{2}$/)) {
        [arrHours, arrMinutes] = arrivalTime.split(':').map(Number);
      } else {
        const arrDate = new Date(arrivalTime);
        arrHours = arrDate.getHours();
        arrMinutes = arrDate.getMinutes();
      }

      // Calculate total minutes
      let totalMinutes =
        arrHours * 60 + arrMinutes - (depHours * 60 + depMinutes);

      // Handle overnight journeys
      if (totalMinutes < 0) {
        totalMinutes += 24 * 60; // Add 24 hours
      }

      const hours = Math.floor(totalMinutes / 60);
      const minutes = totalMinutes % 60;

      return `${hours}h ${minutes}m`;
    } catch (error) {
      console.error('Error calculating duration:', error);
      return 'N/A';
    }
  }

  // Error handling
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An error occurred';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Backend error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }

    console.error(errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
