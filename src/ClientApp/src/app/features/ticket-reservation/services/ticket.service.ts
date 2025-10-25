import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Bus, SearchParams, TrendingRoute } from '../models/ticket.model';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  // BehaviorSubject holds the current state and emits to subscribers
  private searchParamsSubject = new BehaviorSubject<SearchParams | null>(null);
  public searchParams$: Observable<SearchParams | null> = this.searchParamsSubject.asObservable();

  private selectedBusSubject = new BehaviorSubject<Bus | null>(null);
  public selectedBus$: Observable<Bus | null> = this.selectedBusSubject.asObservable();

  constructor() { }

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
    // In a real app, this would be an HTTP call
    const mockBuses: Bus[] = [
      {
        id: '1',
        name: 'Green Line Paribahan',
        from: params.from,
        to: params.to,
        departureTime: '08:00 AM',
        arrivalTime: '02:00 PM',
        fare: 800,
        seatsLeft: 15,
        isAC: true,
        totalSeats: 40,
        bookedSeats: [1, 5, 10, 15, 20]
      },
      {
        id: '2',
        name: 'Shyamoli Paribahan',
        from: params.from,
        to: params.to,
        departureTime: '10:00 AM',
        arrivalTime: '04:00 PM',
        fare: 600,
        seatsLeft: 20,
        isAC: false,
        totalSeats: 40,
        bookedSeats: [2, 7, 12]
      }
    ];

    return new Observable(observer => {
      setTimeout(() => {
        observer.next(mockBuses);
        observer.complete();
      }, 500);
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