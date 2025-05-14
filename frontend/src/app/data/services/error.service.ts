import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ErrorService {
  private errorSubject = new BehaviorSubject<string | null>(null);
  error$ = this.errorSubject.asObservable();

  show(message: string) {
    this.errorSubject.next(message);
  }

  clear() {
    this.errorSubject.next(null);
  }
}
