import { Routes } from '@angular/router';
import { canActivateGuest } from './auth/access.guard';
import { AccessDeniedPageComponent } from './pages/access-denied-page/access-denied-page.component';
import { MainPageComponent } from './pages/main-page/main-page.component';
import { AddToServerPageComponent } from './pages/add-to-server-page/add-to-server-page.component';
import { SteamCallbackComponent } from './common-ui/steam-callback/steam-callback.component';
import { PlayerPageComponent } from './pages/player-page/player-page.component';

export const routes: Routes = [
  {path: '', component: MainPageComponent},
  {path: 'add-to-server', component: AddToServerPageComponent},
  {path: 'player/:id', component: PlayerPageComponent},
  {path: 'steam-callback', component: SteamCallbackComponent, canActivate: [canActivateGuest]},
  {path: 'access-denied', component: AccessDeniedPageComponent }
];
