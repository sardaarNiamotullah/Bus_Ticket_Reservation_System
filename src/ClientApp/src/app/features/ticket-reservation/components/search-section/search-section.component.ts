// // import { Component } from '@angular/core';

// // @Component({
// //   selector: 'app-search-section',
// //   imports: [],
// //   templateUrl: './search-section.component.html',
// //   styleUrl: './search-section.component.css'
// // })
// // export class SearchSectionComponent {

// // }
// import { Component, EventEmitter, OnInit, Output } from '@angular/core';
// import { FormBuilder, FormGroup, Validators } from '@angular/forms';
// import { TicketService } from '../../services/ticket.service';
// import { SearchParams, TrendingRoute } from '../../models/ticket.model';

// @Component({
//   selector: 'app-search-section',
//   templateUrl: './search-section.component.html',
//   styleUrl: './search-section.component.css'
// })
// export class SearchSectionComponent implements OnInit {
//   @Output() searchTriggered = new EventEmitter<SearchParams>();
  
//   searchForm!: FormGroup;
//   trendingRoutes: TrendingRoute[] = [];
//   minDate: string;

//   constructor(
//     private fb: FormBuilder,
//     private ticketService: TicketService
//   ) {
//     // Set minimum date to today
//     const today = new Date();
//     this.minDate = today.toISOString().split('T')[0];
//   }

//   ngOnInit(): void {
//     this.initializeForm();
//     this.loadTrendingRoutes();
//   }

//   private initializeForm(): void {
//     this.searchForm = this.fb.group({
//       from: ['', Validators.required],
//       to: ['', Validators.required],
//       journeyDate: ['', Validators.required]
//     });
//   }

//   private loadTrendingRoutes(): void {
//     this.trendingRoutes = this.ticketService.getTrendingRoutes();
//   }

//   onTrendingRouteClick(route: TrendingRoute): void {
//     this.searchForm.patchValue({
//       from: route.from,
//       to: route.to
//     });
//   }

//   onSearch(): void {
//     if (this.searchForm.valid) {
//       const searchParams: SearchParams = {
//         from: this.searchForm.value.from,
//         to: this.searchForm.value.to,
//         journeyDate: new Date(this.searchForm.value.journeyDate)
//       };
      
//       this.ticketService.setSearchParams(searchParams);
//       this.searchTriggered.emit(searchParams);
//     }
//   }

//   // Swap from and to values
//   swapLocations(): void {
//     const from = this.searchForm.get('from')?.value;
//     const to = this.searchForm.get('to')?.value;
    
//     this.searchForm.patchValue({
//       from: to,
//       to: from
//     });
//   }
// }


import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TicketService } from '../../services/ticket.service';
import { SearchParams, TrendingRoute } from '../../models/ticket.model';

@Component({
  selector: 'app-search-section',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule], // Import dependencies here!
  templateUrl: './search-section.component.html',
  styleUrl: './search-section.component.css'
})
export class SearchSectionComponent implements OnInit {
  @Output() searchTriggered = new EventEmitter<SearchParams>();
  
  searchForm!: FormGroup;
  trendingRoutes: TrendingRoute[] = [];
  minDate: string;

  constructor(
    private fb: FormBuilder,
    private ticketService: TicketService
  ) {
    const today = new Date();
    this.minDate = today.toISOString().split('T')[0];
  }

  ngOnInit(): void {
    this.initializeForm();
    this.loadTrendingRoutes();
  }

  private initializeForm(): void {
    this.searchForm = this.fb.group({
      from: ['', Validators.required],
      to: ['', Validators.required],
      journeyDate: ['', Validators.required]
    });
  }

  private loadTrendingRoutes(): void {
    this.trendingRoutes = this.ticketService.getTrendingRoutes();
  }

  onTrendingRouteClick(route: TrendingRoute): void {
    this.searchForm.patchValue({
      from: route.from,
      to: route.to
    });
  }

  onSearch(): void {
    if (this.searchForm.valid) {
      const searchParams: SearchParams = {
        from: this.searchForm.value.from,
        to: this.searchForm.value.to,
        journeyDate: new Date(this.searchForm.value.journeyDate)
      };
      
      this.ticketService.setSearchParams(searchParams);
      this.searchTriggered.emit(searchParams);
    }
  }

  swapLocations(): void {
    const from = this.searchForm.get('from')?.value;
    const to = this.searchForm.get('to')?.value;
    
    this.searchForm.patchValue({
      from: to,
      to: from
    });
  }
}