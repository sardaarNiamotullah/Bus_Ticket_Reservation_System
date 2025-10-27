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
  styleUrl: './results-section.component.css'
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
      .subscribe(params => {
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
    this.ticketService.searchBuses(params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (buses) => {
          this.buses = buses;
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
        }
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

  formatTime(time: string): string {
    return time;
  }

  getJourneyDuration(departure: string, arrival: string): string {
    // Simple duration calculation
    return '6h 30m';
  }
}