import { Component, inject, OnInit } from '@angular/core';
import { PlayerService } from '../../data/services/player.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { PlayerResponse } from '../../data/interfaces/player.interface';

@Component({
  selector: 'app-player-page',
  imports: [CommonModule],
  templateUrl: './player-page.component.html',
  styleUrl: './player-page.component.scss'
})
export class PlayerPageComponent implements OnInit {
  playerResponse: PlayerResponse | null = null;
  route = inject(ActivatedRoute);
  playerService = inject(PlayerService);
  id:number | null = null;

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.getPlayerDetails(this.id);
  }

  getPlayerDetails(id: number): void {
    this.playerService.getPlayer(id).subscribe({
      next: (data: PlayerResponse) => {
        this.playerResponse = data;
      },
      error: (error) => {
        console.error('Ошибка при загрузке данных игрока', error);
      }
    });
  }
}
