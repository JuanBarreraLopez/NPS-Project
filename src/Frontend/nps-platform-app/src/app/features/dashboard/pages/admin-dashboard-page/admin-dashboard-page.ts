import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService } from '../../../../core/services/dashboard.service';
import { MetricCardComponent } from '../../../../shared/molecules/metric-card/metric-card';
import { RecentVotesListComponent } from '../../../../shared/organisms/recent-votes-list/recent-votes-list';
import { TextLabelComponent } from '../../../../shared/atoms/text-label/text-label';

interface DashboardSummary {
    npsScore: number;
    totalVotes: number;
    promoters: number;
    passives: number;
    detractors: number;
}
interface RecentVote {
    score: number;
    comment: string;
    submittedAt: string;
}

@Component({
  selector: 'app-admin-dashboard-page',
  standalone: true,
  imports: [CommonModule, MetricCardComponent, RecentVotesListComponent, TextLabelComponent],
  templateUrl: './admin-dashboard-page.html',
  styleUrls: ['./admin-dashboard-page.scss']
})
export class AdminDashboardPageComponent implements OnInit {
  private dashboardService = inject(DashboardService);

  summary = signal<DashboardSummary | null>(null);
  recentVotes = signal<RecentVote[]>([]);

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.dashboardService.getSummary().subscribe({
      next: (data: DashboardSummary) => this.summary.set(data),
      error: (err) => console.error('Error cargando resumen:', err) 
    });

    this.dashboardService.getRecentVotes().subscribe({
      next: (data: RecentVote[]) => this.recentVotes.set(data),
      error: (err) => console.error('Error cargando votos recientes:', err)
    });
  }
}