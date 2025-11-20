import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { VoteService } from '../../../../core/services/vote';
import { AuthService } from '../../../../core/services/auth.service';
import { ButtonComponent } from '../../../../shared/atoms/button/button';

@Component({
  selector: 'app-vote-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent],
  templateUrl: './vote-page.html',
  styleUrls: ['./vote-page.scss'],
})
export class VotePageComponent {
  private voteService = inject(VoteService);
  private authService = inject(AuthService);
  private router = inject(Router);
  scale = Array.from({ length: 11 }, (_, i) => i);

  selectedScore = signal<number | null>(null);
  comment = signal('');
  loading = signal(false);
  hasVoted = signal(false);
  errorMessage = signal('');

  setScore(val: number) {
    this.selectedScore.set(val);
  }

  submit() {
    const score = this.selectedScore();
    if (score === null) return;

    this.loading.set(true);
    this.errorMessage.set('');

    this.voteService.submitVote(score, this.comment()).subscribe({
      next: () => {
        this.hasVoted.set(true);
        this.loading.set(false);
        setTimeout(() => {
          this.authService.logout();
        }, 3000);
      },
      error: (err) => {
        console.error(err);
        this.loading.set(false);
        if (err.status === 409) {
          this.errorMessage.set('Ya has registrado tu voto anteriormente.');
        } else {
          this.errorMessage.set('Error al enviar el voto. Intente nuevamente.');
        }
      },
    });
  }
}
