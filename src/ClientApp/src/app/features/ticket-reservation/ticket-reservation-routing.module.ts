import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TicketReservationPageComponent } from './components/ticket-reservation-page/ticket-reservation-page.component';

const routes: Routes = [
  {
    path: '',
    component: TicketReservationPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TicketReservationRoutingModule { }