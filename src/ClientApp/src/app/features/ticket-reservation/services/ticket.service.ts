import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Bus, SearchParams, TrendingRoute } from '../models/ticket.model';

@Injectable({
  providedIn: 'root',
})
export class TicketService {
  // BehaviorSubject holds the current state and emits to subscribers
  private searchParamsSubject = new BehaviorSubject<SearchParams | null>(null);
  public searchParams$: Observable<SearchParams | null> =
    this.searchParamsSubject.asObservable();

  private selectedBusSubject = new BehaviorSubject<Bus | null>(null);
  public selectedBus$: Observable<Bus | null> =
    this.selectedBusSubject.asObservable();

  constructor() {}

  // Mock data for trending routes
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

  // Mock data for available buses
  searchBuses(params: SearchParams): Observable<Bus[]> {
    const mockBuses: Bus[] = [
      {
        id: 'SS-DHA-C416',
        name: 'National Travels',
        from: params.from,
        to: params.to,
        departureTime: '6:00 AM',
        arrivalTime: '1:30 PM',
        fare: 700,
        seatsLeft: 36,
        isAC: false,
        totalSeats: 40,
        bookedSeats: [1, 5, 10, 15],
      },
      {
        id: '10-MHL-C1SAP',
        name: 'National Travels',
        from: params.from,
        to: params.to,
        departureTime: '6:00 AM',
        arrivalTime: '1:30 PM',
        fare: 700,
        seatsLeft: 40,
        isAC: false,
        totalSeats: 40,
        bookedSeats: [],
      },
      {
        id: 'GS-RAJ-CZA401',
        name: 'Hanif Enterprise',
        from: params.from,
        to: params.to,
        departureTime: '06:00 AM',
        arrivalTime: '12:30 PM',
        fare: 700,
        seatsLeft: 40,
        isAC: false,
        totalSeats: 40,
        bookedSeats: [],
      },
      {
        id: '303 DHK-CHAP',
        name: 'Grameen Travels',
        from: params.from,
        to: params.to,
        departureTime: '6:01 AM',
        arrivalTime: '12:51 PM',
        fare: 700,
        seatsLeft: 36,
        isAC: false,
        totalSeats: 40,
        bookedSeats: [2, 7, 12, 18],
      },
      {
        id: 'SL-DHA-001',
        name: 'Shyamoli Paribahan',
        from: params.from,
        to: params.to,
        departureTime: '10:00 AM',
        arrivalTime: '04:30 PM',
        fare: 600,
        seatsLeft: 8,
        isAC: false,
        totalSeats: 40,
        bookedSeats: [1, 2, 3, 5, 7, 8, 10, 12, 14, 15, 16, 18, 20, 22, 24, 25],
      },
      {
        id: 'GL-AC-456',
        name: 'Green Line Paribahan',
        from: params.from,
        to: params.to,
        departureTime: '08:00 AM',
        arrivalTime: '02:00 PM',
        fare: 800,
        seatsLeft: 15,
        isAC: true,
        totalSeats: 40,
        bookedSeats: [1, 5, 10, 15, 20],
      },
    ];

    return new Observable((observer) => {
      setTimeout(() => {
        observer.next(mockBuses);
        observer.complete();
      }, 800);
    });
  }

  // Set selected bus for booking
  setSelectedBus(bus: Bus): void {
    this.selectedBusSubject.next(bus);
  }

  // Get selected bus
  getSelectedBus(): Bus | null {
    return this.selectedBusSubject.value;
  }
}
