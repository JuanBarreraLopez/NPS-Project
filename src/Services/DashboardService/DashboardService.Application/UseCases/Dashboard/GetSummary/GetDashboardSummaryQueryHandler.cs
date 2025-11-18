using System;
using System.Collections.Generic;
using System.Text;
using DashboardService.Application.Interfaces;
using DashboardService.Domain;
using MediatR;

namespace DashboardService.Application.UseCases.Dashboard.GetSummary
{
    public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
    {
        private readonly IDashboardRepository _dashboardRepository;

        public GetDashboardSummaryQueryHandler(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
        {
            // 1. Llama al "contrato" (la interfaz)
            var summary = await _dashboardRepository.GetDashboardSummaryAsync();

            // 2. Devuelve el resultado
            return summary;
        }
    }
}