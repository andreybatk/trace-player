import { Component, inject, OnInit } from '@angular/core';
import { PlayerService } from '../../data/services/player.service';
import { PlayerItem } from '../../data/interfaces/player.interface';
import { NgxPaginationModule } from 'ngx-pagination';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-player-list',
  imports: [NgxPaginationModule, CommonModule, FormsModule, RouterModule],
  templateUrl: './player-list.component.html',
  styleUrl: './player-list.component.scss'
})
export class PlayerListComponent implements OnInit {
  playerService = inject(PlayerService);
  players: PlayerItem[] = [];;
  totalCount = 0;
  page = 1;
  pageSize = 30;
  searchSteamId: string = '';

  updatePaginationConfig() {
    this.paginationConfig = {
      id: 'pagination',
      itemsPerPage: this.pageSize,
      currentPage: this.page,
      totalItems: this.totalCount,
      previousLabel: 'Назад',
      nextLabel: 'Вперед'
    };
  }

  paginationConfig = {
    id: 'pagination',
    itemsPerPage: this.pageSize,
    currentPage: this.page,
    totalItems: this.totalCount,
    previousLabel: 'Назад',
    nextLabel: 'Вперед'
  };

  ngOnInit(): void {
    this.loadPlayers();
  }

  loadPlayers(): void {
    this.playerService.getPlayersPaginated(this.searchSteamId, this.page, this.pageSize).subscribe(response => {
      this.players = response.players;
      this.totalCount = response.totalCount;
      this.updatePaginationConfig();
    });
  }

  onPageChange(page: number): void {
    this.page = page;
    this.loadPlayers();
  }

  onSearchChange(): void {
    this.page = 1;
    this.loadPlayers();
  }
}
