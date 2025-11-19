import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TextLabelComponent } from '../../atoms/text-label/text-label';

interface RecentVote {
    score: number;
    comment: string;
    submittedAt: string; 
}

@Component({
  selector: 'app-recent-votes-list',
  standalone: true,
  imports: [CommonModule, TextLabelComponent],
  templateUrl: './recent-votes-list.html',
  styleUrls: ['./recent-votes-list.scss']
})
export class RecentVotesListComponent {
  @Input() votes: RecentVote[] = [];

  formatTimestamp(dateString: string): string {
    return new Date(dateString).toLocaleDateString('es-CO', { year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' });
  }
}