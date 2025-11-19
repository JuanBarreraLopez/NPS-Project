import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-text-label',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './text-label.html',
  styleUrls: ['./text-label.scss']
})
export class TextLabelComponent {
  @Input() text: string = '';
  @Input() level: 'h1' | 'h2' | 'label' | 'score' = 'label';
}
