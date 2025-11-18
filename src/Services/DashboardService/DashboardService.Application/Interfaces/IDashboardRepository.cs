using System;
using System.Collections.Generic;
using System.Text;
using DashboardService.Domain;

namespace DashboardService.Application.Interfaces
{
    public interface IDashboardRepository
    {
        // Contrato para el Resumen
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();

        // Contrato para los Votos Recientes
        Task<IEnumerable<RecentVoteDto>> GetRecentVotesAsync(int limit = 10);
    }
}