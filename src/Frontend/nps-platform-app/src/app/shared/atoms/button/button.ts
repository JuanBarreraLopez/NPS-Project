import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './button.html',
  styleUrls: ['./button.scss'],
})
export class ButtonComponent {
  @Input() appearance: 'primary' | 'secondary' = 'primary';

  @Input() type: 'button' | 'submit' = 'submit';

  @Input() disabled: boolean = false;

  @Input() loading: boolean = false;
}
