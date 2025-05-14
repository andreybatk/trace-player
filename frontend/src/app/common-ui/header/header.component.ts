import { Component, HostListener, ElementRef, inject, OnInit } from '@angular/core';
import { AuthService } from '../../auth/auth.service';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterModule } from '@angular/router';
import { PlayerService } from '../../data/services/player.service';

@Component({
  selector: 'app-header',
  imports: [CommonModule, RouterModule, RouterLink, RouterLinkActive],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  authService = inject(AuthService);
  playerService = inject(PlayerService);
  router = inject(Router);
  eRef:ElementRef = inject(ElementRef);
  isDropdownOpen = false;

  goToProfile() {
    this.playerService.getIdByAuth().subscribe({
      next: (playerId) => {
        if (playerId !== null && playerId !== undefined) {
          this.router.navigate(['/player', playerId]);
        }
      },
      error: (err) => {
      }
    });
  }

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event) {
    if (!this.eRef.nativeElement.contains(event.target)) {
      this.isDropdownOpen = false;
    }
  }
}
