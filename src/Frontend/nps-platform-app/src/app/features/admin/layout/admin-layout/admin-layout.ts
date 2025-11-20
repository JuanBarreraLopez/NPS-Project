import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { SideMenuComponent } from '../../../../shared/organisms/side-menu/side-menu';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, SideMenuComponent],
  templateUrl: './admin-layout.html',
  styleUrls: ['./admin-layout.scss'],
})
export class AdminLayoutComponent {}
