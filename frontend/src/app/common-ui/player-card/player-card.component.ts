import { Component, Input } from '@angular/core';
import { PlayerResponse } from '../../data/interfaces/player.interface';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-player-card',
  imports: [CommonModule],
  templateUrl: './player-card.component.html',
  styleUrl: './player-card.component.scss'
})
export class PlayerCardComponent {
 @Input() playerResponse: PlayerResponse | null = null;
}
