import { Component } from '@angular/core';
import { NgxPaginationModule } from 'ngx-pagination';
import { CommonModule } from '@angular/common';
import { PlayerListComponent } from '../../common-ui/player-list/player-list.component';

@Component({
  selector: 'app-main-page',
  imports: [NgxPaginationModule, CommonModule, PlayerListComponent],
  templateUrl: './main-page.component.html',
  styleUrl: './main-page.component.scss'
})
export class MainPageComponent {
 
}
