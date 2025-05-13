import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-not-found',
  imports: [RouterLink],
  templateUrl: './not-found.component.html',
  styleUrl: './not-found.component.scss'
})
export class NotFoundComponent {
  message: string = 'Страница не найдена.';

  constructor() {
    const state = window.history.state;
    if (state && state.message) {
      this.message = state.message;
    }
  }
}
