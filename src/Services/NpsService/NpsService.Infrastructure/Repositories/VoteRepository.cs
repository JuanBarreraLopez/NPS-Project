using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using NpsService.Application.Interfaces;
using NpsService.Domain;
using System.Data;

namespace NpsService.Infrastructure.Repositories
{
    /**
     * Implementación del repositorio de Votos usando Dapper.
     * Implementa el contrato de 'IVoteRepository'.
     */
    public class VoteRepository : IVoteRepository
    {
        private readonly IDbConnection _connection;

        public VoteRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        // --- Implementación de los métodos del contrato ---

        public async Task AddAsync(Vote vote)
        {
            var sql = @"
                INSERT INTO Votes (Id, UserId, Score, Comment, CreatedAtUtc)
                VALUES (@Id, @UserId, @Score, @Comment, @CreatedAtUtc)";
            await _connection.ExecuteAsync(sql, vote);
        }

        public async Task<bool> HasUserVotedAsync(Guid userId)
        {
            var sql = "SELECT COUNT(1) FROM Votes WHERE UserId = @UserId";
            var count = await _connection.ExecuteScalarAsync<int>(sql, new { UserId = userId });
            return count > 0;
        }

        public async Task<IEnumerable<Vote>> GetAllAsync()
        {
            var sql = "SELECT * FROM Votes";
            return await _connection.QueryAsync<Vote>(sql);
        }
    }
}