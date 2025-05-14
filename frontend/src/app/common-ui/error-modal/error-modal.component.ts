import { Component } from '@angular/core';
import { ErrorService } from '../../data/services/error.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-error-modal',
  imports: [CommonModule],
  templateUrl: './error-modal.component.html',
  styleUrl: './error-modal.component.scss'
})
export class ErrorModalComponent {
  message: string | null = null;

  constructor(private errorService: ErrorService) {
    this.errorService.error$.subscribe(msg => this.message = msg);
  }

  close() {
    this.errorService.clear();
  }
}
