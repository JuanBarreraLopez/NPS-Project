using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DashboardService.Application.Interfaces;
using DashboardService.Domain;
using MediatR;

namespace DashboardService.Application.UseCases.Dashboard.GetRecentVotes
{
    public class GetRecentVotesQueryHandler : IRequestHandler<GetRecentVotesQuery, IEnumerable<RecentVoteDto>>
    {
        private readonly IDashboardRepository _dashboardRepository;

        public GetRecentVotesQueryHandler(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<IEnumerable<RecentVoteDto>> Handle(GetRecentVotesQuery request, CancellationToken cancellationToken)
        {
            var recentVotes = await _dashboardRepository.GetRecentVotesAsync(10);

            return recentVotes;
        }
    }
}