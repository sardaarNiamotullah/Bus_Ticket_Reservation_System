import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SearchParams } from '../../models/ticket.model';
import { SearchSectionComponent } from '../search-section/search-section.component';
import { ResultsSectionComponent } from '../results-section/results-section.component';
import { BookingSectionComponent } from '../booking-section/booking-section.component';

@Component({
  selector: 'app-ticket-reservation-page',
  standalone: true,
  imports: [
    CommonModule,
    SearchSectionComponent,
    ResultsSectionComponent,
    BookingSectionComponent
  ], // Import child components!
  templateUrl: './ticket-reservation-page.component.html',
  styleUrl: './ticket-reservation-page.component.css'
})
export class TicketReservationPageComponent {
  
  onSearchTriggered(searchParams: SearchParams): void {
    console.log('Search triggered with params:', searchParams);
  }
}