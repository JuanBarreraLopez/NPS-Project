using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DashboardService.Domain;
using MediatR;

namespace DashboardService.Application.UseCases.Dashboard.GetRecentVotes
{
    public class GetRecentVotesQuery : IRequest<IEnumerable<RecentVoteDto>>
    {

    }
}