using System;
using System.Collections.Generic;
using System.Text;
using DashboardService.Domain;
using MediatR;

namespace DashboardService.Application.UseCases.Dashboard.GetSummary
{
    public class GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>
    {
    }
}