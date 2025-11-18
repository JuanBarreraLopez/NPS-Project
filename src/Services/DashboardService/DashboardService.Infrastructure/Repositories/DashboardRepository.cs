using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DashboardService.Application.Interfaces;
using DashboardService.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;

namespace DashboardService.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("VotesDb");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var sql = @"
                SELECT
                    COUNT(*) AS TotalVotes,
                    
                    SUM(CASE WHEN Score >= 9 THEN 1 ELSE 0 END) AS PromoterCount,
                    
                    SUM(CASE WHEN Score <= 6 THEN 1 ELSE 0 END) AS DetractorCount,
                    
                    SUM(CASE WHEN Score BETWEEN 7 AND 8 THEN 1 ELSE 0 END) AS NeutralCount
                
                FROM Votes;
            ";

            using (var connection = CreateConnection())
            {
                var summary = await connection.QuerySingleOrDefaultAsync<DashboardSummaryDto>(sql);

                if (summary != null && summary.TotalVotes > 0)
                {
                    summary.NpsScore =
                        ((double)(summary.PromoterCount - summary.DetractorCount) / summary.TotalVotes) * 100.0;
                }

                return summary ?? new DashboardSummaryDto();
            }
        }

        public async Task<IEnumerable<RecentVoteDto>> GetRecentVotesAsync(int limit = 10)
        {
            var sql = @"
                SELECT TOP (@Limit)
                    Score,
                    Comment,
                    CreatedAtUtc AS SubmittedAt 
                FROM Votes
                ORDER BY CreatedAtUtc DESC;   
            ";

            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<RecentVoteDto>(sql, new { Limit = limit });
            }
        }
    }
}