import { Component, inject, OnInit } from '@angular/core';
import { PlayerService } from '../../data/services/player.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { PlayerResponse } from '../../data/interfaces/player.interface';
import { PlayerCardComponent } from '../../common-ui/player-card/player-card.component';
import { LoaderComponent } from '../../common-ui/loader/loader.component';

@Component({
  selector: 'app-player-page',
  imports: [CommonModule, PlayerCardComponent, LoaderComponent],
  templateUrl: './player-page.component.html',
  styleUrl: './player-page.component.scss'
})
export class PlayerPageComponent implements OnInit {
  playerResponse: PlayerResponse | null = null;
  route = inject(ActivatedRoute);
  playerService = inject(PlayerService);
  id:number | null = null;
  isLoading: boolean = true;

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.getPlayerById(this.id);
  }

  getPlayerById(id: number): void {
    this.playerService.getPlayer(id).subscribe({
      next: (data: PlayerResponse) => {
        this.playerResponse = data;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Ошибка при загрузке данных игрока', error);
      }
    });
  }
}
