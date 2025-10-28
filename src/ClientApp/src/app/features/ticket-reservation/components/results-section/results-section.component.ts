import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';
import { TicketService } from '../../services/ticket.service';
import { Bus, SearchParams } from '../../models/ticket.model';

@Component({
  selector: 'app-results-section',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './results-section.component.html',
  styleUrl: './results-section.component.css',
})
export class ResultsSectionComponent implements OnInit, OnDestroy {
  buses: Bus[] = [];
  isLoading = false;
  searchParams: SearchParams | null = null;
  private destroy$ = new Subject<void>();

  constructor(private ticketService: TicketService) {}

  ngOnInit(): void {
    // Subscribe to search params changes
    this.ticketService.searchParams$
      .pipe(takeUntil(this.destroy$))
      .subscribe((params) => {
        if (params) {
          this.searchParams = params;
          this.searchBuses(params);
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private searchBuses(params: SearchParams): void {
    this.isLoading = true;
    this.ticketService
      .searchBuses(params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (buses) => {
          this.buses = buses;
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
        },
      });
  }

  onViewSeats(bus: Bus): void {
    this.ticketService.setSelectedBus(bus);
    // Scroll to booking section
    setTimeout(() => {
      const bookingSection = document.querySelector('app-booking-section');
      if (bookingSection) {
        bookingSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    }, 100);
  }

  getJourneyDuration(departure: string, arrival: string): string {
    // Split into hours/minutes
    const [depH, depM] = departure.split(':').map(Number);
    const [arrH, arrM] = arrival.split(':').map(Number);

    // Convert both to minutes since midnight
    let depMinutes = depH * 60 + depM;
    let arrMinutes = arrH * 60 + arrM;

    // Handle overnight trips (arrival next day)
    if (arrMinutes < depMinutes) {
      arrMinutes += 24 * 60;
    }

    const diff = arrMinutes - depMinutes;
    const hours = Math.floor(diff / 60);
    const minutes = diff % 60;

    return `${hours}h ${minutes}m`;
  }

  formatTime(time: string): string {
    return time;
  }
}
