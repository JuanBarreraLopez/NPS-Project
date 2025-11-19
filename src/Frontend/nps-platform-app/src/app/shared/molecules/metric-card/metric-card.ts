import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TextLabelComponent } from '../../atoms/text-label/text-label';

@Component({
  selector: 'app-metric-card',
  standalone: true,
  imports: [CommonModule, TextLabelComponent],
  templateUrl: './metric-card.html',
  styleUrls: ['./metric-card.scss']
})
export class MetricCardComponent {
  @Input() label: string = 'Métrica';
  @Input() value: string = 'N/A';
  @Input() subtext: string = '';
  @Input() valueLevel: 'h2' | 'score' = 'h2';
}