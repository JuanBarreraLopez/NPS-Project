using System;
using System.Collections.Generic;
using System.Text;

namespace DashboardService.Domain
{
    public class RecentVoteDto
    {
        public int Score { get; set; }
        public string Comment { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}