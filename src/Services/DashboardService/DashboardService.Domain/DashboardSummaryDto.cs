using System;
using System.Collections.Generic;
using System.Text;

namespace DashboardService.Domain
{
    public class DashboardSummaryDto
    {
        public int TotalVotes { get; set; }
        public int PromoterCount { get; set; }
        public int DetractorCount { get; set; }
        public int NeutralCount { get; set; }
        public double NpsScore { get; set; }
    }
}