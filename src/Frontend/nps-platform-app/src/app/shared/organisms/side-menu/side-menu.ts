import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-side-menu',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './side-menu.html',
  styleUrls: ['./side-menu.scss'],
})
export class SideMenuComponent {
  private authService = inject(AuthService);

  username = 'Administrador';

  logout() {
    this.authService.logout();
  }
}
