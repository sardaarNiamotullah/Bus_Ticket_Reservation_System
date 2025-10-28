import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { TicketService } from '../../services/ticket.service';
import { Bus, Seat } from '../../models/ticket.model';

@Component({
  selector: 'app-booking-section',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './booking-section.component.html',
  styleUrl: './booking-section.component.css'
})
export class BookingSectionComponent implements OnInit, OnDestroy {
  selectedBus: Bus | null = null;
  seats: Seat[] = [];
  bookingForm!: FormGroup;
  selectedSeats: number[] = [];
  isLoadingSeats = false;
  isBooking = false;
  errorMessage: string | null = null;

  private destroy$ = new Subject<void>();

  constructor(
    private ticketService: TicketService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    
    // Subscribe to selected bus changes
    this.ticketService.selectedBus$
      .pipe(takeUntil(this.destroy$))
      .subscribe(bus => {
        if (bus) {
          this.selectedBus = bus;
          this.loadSeatsFromBackend();
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeForm(): void {
    this.bookingForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]]
    });
  }

  // Load actual seats from backend
  private loadSeatsFromBackend(): void {
    if (!this.selectedBus) return;

    this.isLoadingSeats = true;
    this.errorMessage = null;
    this.seats = [];
    this.selectedSeats = [];

    this.ticketService.getSeatPlan(this.selectedBus.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (seats) => {
          this.seats = seats;
          this.isLoadingSeats = false;
        },
        error: (error) => {
          console.error('Error loading seats:', error);
          this.errorMessage = 'Failed to load seat information. Please try again.';
          this.isLoadingSeats = false;
        }
      });
  }

  toggleSeat(seat: Seat): void {
    if (seat.isBooked || seat.isSold) return;

    seat.isSelected = !seat.isSelected;

    if (seat.isSelected) {
      this.selectedSeats.push(seat.number);
    } else {
      this.selectedSeats = this.selectedSeats.filter(num => num !== seat.number);
    }

    // Sort selected seats
    this.selectedSeats.sort((a, b) => a - b);
  }

  getSeatClass(seat: Seat): string {
    if (seat.isSelected) return 'bg-green-500 text-white cursor-pointer hover:bg-green-600';
    if (seat.isSold) return 'bg-red-400 text-white cursor-not-allowed';
    if (seat.isBooked) return 'bg-orange-400 text-white cursor-not-allowed';
    return 'bg-white border-2 border-gray-300 text-gray-700 cursor-pointer hover:bg-green-100 hover:border-green-500';
  }

  getTotalAmount(): number {
    if (!this.selectedBus) return 0;
    return this.selectedSeats.length * this.selectedBus.fare;
  }

  onBook(): void {
    this.processBooking('Book');
  }

  onBuy(): void {
    this.processBooking('Buy');
  }

  private processBooking(bookingType: 'Book' | 'Buy'): void {
    if (this.bookingForm.invalid || this.selectedSeats.length === 0) {
      this.markFormAsTouched();
      return;
    }

    if (!this.selectedBus) {
      alert('Please select a bus first');
      return;
    }

    this.isBooking = true;
    this.errorMessage = null;

    const bookingDetails = {
      scheduleId: this.selectedBus.id,
      seatNumbers: this.selectedSeats,
      passengerName: this.bookingForm.value.name,
      passengerEmail: this.bookingForm.value.email,
      bookingType: bookingType
    };

    this.ticketService.bookSeats(bookingDetails)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.isBooking = false;
          const action = bookingType === 'Book' ? 'reserved' : 'purchased';
          alert(
            `Success!\n\nSeats ${this.selectedSeats.join(', ')} have been ${action}!\n` +
            `Booking ID: ${response.bookingId}\n` +
            `Total Amount: ৳${response.totalAmount}\n\n` +
            `${response.message}`
          );
          this.resetBooking();
          // Reload seats to show updated availability
          this.loadSeatsFromBackend();
        },
        error: (error) => {
          this.isBooking = false;
          console.error('Booking error:', error);
          this.errorMessage = error.message || 'Failed to complete booking. Please try again.';
        }
      });
  }

  private markFormAsTouched(): void {
    Object.keys(this.bookingForm.controls).forEach(key => {
      this.bookingForm.get(key)?.markAsTouched();
    });
  }

  private resetBooking(): void {
    this.bookingForm.reset();
    this.selectedSeats = [];
    this.seats.forEach(seat => seat.isSelected = false);
  }

  // Helper method to split seats into rows of 4 (2 + 2)
  getSeatRows(): Seat[][] {
    const rows: Seat[][] = [];
    for (let i = 0; i < this.seats.length; i += 4) {
      rows.push(this.seats.slice(i, i + 4));
    }
    return rows;
  }
}