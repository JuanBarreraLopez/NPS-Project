using IdentityService.Application.Interfaces;
using IdentityService.Domain;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace IdentityService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var sql = "SELECT * FROM Users WHERE Username = @Username";
            return await _connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            var sql = "SELECT * FROM Users WHERE Id = @UserId";
            return await _connection.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
        }

        public async Task AddAsync(User user)
        {
            var sql = @"
                INSERT INTO Users (Id, Username, PasswordHash, PasswordSalt, Role, IsBlocked, FailedAttempts, LastActivityUtc, CreatedAtUtc)
                VALUES (@Id, @Username, @PasswordHash, @PasswordSalt, @Role, @IsBlocked, @FailedAttempts, @LastActivityUtc, @CreatedAtUtc)";
            await _connection.ExecuteAsync(sql, user);
        }

        public async Task UpdateAsync(User user)
        {
            var sql = @"
                UPDATE Users 
                SET 
                    PasswordHash = @PasswordHash,
                    PasswordSalt = @PasswordSalt,
                    Role = @Role,
                    IsBlocked = @IsBlocked,
                    FailedAttempts = @FailedAttempts,
                    LastActivityUtc = @LastActivityUtc
                WHERE Id = @Id";
            await _connection.ExecuteAsync(sql, user);
        }
    }
}