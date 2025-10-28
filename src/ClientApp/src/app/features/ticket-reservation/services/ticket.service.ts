import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import {
  Bus,
  SearchParams,
  TrendingRoute,
  Seat,
} from '../models/ticket.model';

interface BackendBus {
  id: string;
  busId: string;
  name: string;
  from: string;
  to: string;
  departureTime: string;
  arrivalTime: string;
  fare: number;
  seatsLeft: number;
  isAC: boolean;
  totalSeats: number;
  journeyDate: string;
}

interface BackendSeatPlan {
  scheduleId: string;
  busName: string;
  route: string;
  journeyDate: string;
  totalSeats: number;
  availableSeats: number;
  seats: Array<{
    id: string;
    number: number;
    row: number;
    status: string;
    isBooked: boolean;
    isSold: boolean;
  }>;
}

interface BackendBookingRequest {
  scheduleId: string;
  seatNumbers: number[];
  passengerName: string;
  passengerEmail: string;
  bookingType: string;
}

interface BackendBookingResponse {
  bookingIds: string[];
  scheduleId: string;
  seatNumbers: number[];
  passengerName: string;
  passengerEmail: string;
  bookingType: string;
  bookingDate: string;
  message: string;
  success: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class TicketService {
  private apiUrl = 'http://localhost:5123/api';

  private searchParamsSubject = new BehaviorSubject<SearchParams | null>(null);
  public searchParams$: Observable<SearchParams | null> =
    this.searchParamsSubject.asObservable();

  private selectedBusSubject = new BehaviorSubject<Bus | null>(null);
  public selectedBus$: Observable<Bus | null> =
    this.selectedBusSubject.asObservable();

  constructor(private http: HttpClient) {}

  getTrendingRoutes(): TrendingRoute[] {
    return [
      { from: 'Dhaka', to: 'Chittagong', popular: true },
      { from: 'Dhaka', to: 'Sylhet', popular: true },
      { from: 'Dhaka', to: 'Rajshahi', popular: true },
      { from: 'Dhaka', to: 'Khulna', popular: true },
    ];
  }

  setSearchParams(params: SearchParams): void {
    this.searchParamsSubject.next(params);
  }

  getSearchParams(): SearchParams | null {
    return this.searchParamsSubject.value;
  }

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

  getSeatPlan(scheduleId: string): Observable<Seat[]> {
    console.log('Fetching seat plan for schedule:', scheduleId);
    return this.http
      .get<BackendSeatPlan>(`${this.apiUrl}/Booking/seat-plan/${scheduleId}`)
      .pipe(
        map((response) => {
          console.log('Backend seat plan response:', response);
          const seats = this.mapBackendSeatsToFrontend(response.seats);
          console.log('Mapped seats:', seats);
          return seats;
        }),
        catchError(this.handleError)
      );
  }

  bookSeats(bookingDetails: {
    scheduleId: string;
    seatNumbers: number[];
    passengerName: string;
    passengerEmail: string;
    bookingType: 'Book' | 'Buy';
  }): Observable<any> {
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
        map((response) => ({
          bookingId: response.bookingIds[0],
          scheduleId: response.scheduleId,
          seatNumbers: response.seatNumbers,
          totalAmount: bookingDetails.seatNumbers.length * (this.getSelectedBus()?.fare || 0),
          status: response.success ? 'Confirmed' : 'Failed',
          message: response.message,
        })),
        tap((response) => console.log('Booking response:', response)),
        catchError(this.handleError)
      );
  }

  setSelectedBus(bus: Bus): void {
    this.selectedBusSubject.next(bus);
  }

  getSelectedBus(): Bus | null {
    return this.selectedBusSubject.value;
  }

  private mapBackendBusesToFrontend(
    backendBuses: BackendBus[],
    params: SearchParams
  ): Bus[] {
    return backendBuses.map((bus) => ({
      id: bus.id,
      name: bus.name,
      from: params.from,
      to: params.to,
      departureTime: this.formatTime(bus.departureTime),
      arrivalTime: this.formatTime(bus.arrivalTime),
      fare: bus.fare,
      seatsLeft: bus.seatsLeft,
      isAC: bus.isAC,
      totalSeats: bus.totalSeats,
      bookedSeats: [],
    }));
  }

  private mapBackendSeatsToFrontend(
    backendSeats: Array<{
      id: string;
      number: number;
      row: number;
      status: string;
      isBooked: boolean;
      isSold: boolean;
    }>
  ): Seat[] {
    return backendSeats.map((seat) => ({
      number: seat.number,
      isBooked: seat.isBooked,
      isSold: seat.isSold,
      isSelected: false,
    }));
  }

  private formatTime(timeString: string): string {
    if (!timeString) return 'N/A';

    // Check if it's already in HH:MM format from backend
    if (timeString.match(/^\d{1,2}:\d{2}$/)) {
      const [hours, minutes] = timeString.split(':').map(Number);
      const ampm = hours >= 12 ? 'PM' : 'AM';
      const displayHours = hours % 12 || 12;
      const minutesStr = minutes < 10 ? '0' + minutes : minutes.toString();
      return `${displayHours}:${minutesStr} ${ampm}`;
    }

    // If it's HH:MM:SS format
    if (timeString.match(/^\d{1,2}:\d{2}:\d{2}$/)) {
      const [hours, minutes] = timeString.split(':').map(Number);
      const ampm = hours >= 12 ? 'PM' : 'AM';
      const displayHours = hours % 12 || 12;
      const minutesStr = minutes < 10 ? '0' + minutes : minutes.toString();
      return `${displayHours}:${minutesStr} ${ampm}`;
    }

    // If it's ISO string, parse it
    try {
      const date = new Date(timeString);
      let hours = date.getHours();
      const minutes = date.getMinutes();
      const ampm = hours >= 12 ? 'PM' : 'AM';
      hours = hours % 12;
      hours = hours ? hours : 12;
      const minutesStr = minutes < 10 ? '0' + minutes : minutes.toString();
      return `${hours}:${minutesStr} ${ampm}`;
    } catch (error) {
      console.error('Error formatting time:', error);
      return timeString;
    }
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An error occurred';

    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
      if (error.error && error.error.message) {
        errorMessage = error.error.message;
      }
    }

    console.error('HTTP Error:', errorMessage, error);
    return throwError(() => new Error(errorMessage));
  }
}