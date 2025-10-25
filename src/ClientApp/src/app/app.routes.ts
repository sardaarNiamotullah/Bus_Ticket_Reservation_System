import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'ticket-reservation',
    pathMatch: 'full'
  },
  {
    path: 'ticket-reservation',
    loadChildren: () => import('./features/ticket-reservation/ticket-reservation.module').then(m => m.TicketReservationModule)
  }
];