import { Component } from '@angular/core';
import { GreetingComponent } from './features/greeting/presentation/components/greeting/greeting.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [GreetingComponent],
  template: '<app-greeting></app-greeting>',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'ClientApp';
}
