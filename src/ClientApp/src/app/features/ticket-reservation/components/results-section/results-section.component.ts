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
    try {
      // Parse time strings like "10:30 PM" or "22:00"
      const parseTime = (timeStr: string): number => {
        // Check if it's in 12-hour format (e.g., "10:30 PM")
        if (timeStr.includes('AM') || timeStr.includes('PM')) {
          const [time, period] = timeStr.split(' ');
          let [hours, minutes] = time.split(':').map(Number);

          if (period === 'PM' && hours !== 12) {
            hours += 12;
          } else if (period === 'AM' && hours === 12) {
            hours = 0;
          }

          return hours * 60 + minutes;
        }

        // If it's in 24-hour format (e.g., "22:00")
        const [hours, minutes] = timeStr.split(':').map(Number);
        return hours * 60 + minutes;
      };

      const depMinutes = parseTime(departure);
      let arrMinutes = parseTime(arrival);

      // Handle overnight trips (arrival next day)
      if (arrMinutes < depMinutes) {
        arrMinutes += 24 * 60;
      }

      const diff = arrMinutes - depMinutes;
      const hours = Math.floor(diff / 60);
      const minutes = diff % 60;

      return `${hours}h ${minutes}m`;
    } catch (error) {
      console.error('Error calculating duration:', error, {
        departure,
        arrival,
      });
      return 'N/A';
    }
  }

  formatTime(time: string): string {
    return time;
  }
}
