import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GreetingService } from '../../../application/greeting.service';
import { GreetingEntity } from '../../../domain/models/greeting.model';

@Component({
  selector: 'app-greeting',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './greeting.component.html',
  styleUrl: './greeting.component.css'
})
export class GreetingComponent implements OnInit {
  greeting?: GreetingEntity;

  constructor(private greetingService: GreetingService) {}

  ngOnInit(): void {
    this.greetingService.getGreeting().subscribe({
      next: (greeting) => {
        this.greeting = greeting;
      }
    });
  }
}