import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { GreetingEntity } from '../domain/models/greeting.model';

@Injectable({
  providedIn: 'root'
})
export class GreetingService {
  getGreeting(): Observable<GreetingEntity> {
    return of(GreetingEntity.createHelloWorld());
  }
}