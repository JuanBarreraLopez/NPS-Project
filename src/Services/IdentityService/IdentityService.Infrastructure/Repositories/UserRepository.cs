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
    /**
     * Implementación del repositorio de usuarios.
     * Utiliza Dapper para comunicarse con SQL Server.
     * Esta clase implementa el contrato 'IUserRepository' de la capa 'Application'.
     */
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository()
        {
            // ¡Placeholder! Esto lo configuraremos globalmente después.
            _connectionString = "Server=...;Database=IdentityDB;...;";
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using (var connection = CreateConnection())
            {
                var sql = "SELECT * FROM Users WHERE Username = @Username";
                return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
            }
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            using (var connection = CreateConnection())
            {
                var sql = "SELECT * FROM Users WHERE Id = @UserId";
                return await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
            }
        }

        public async Task AddAsync(User user)
        {
            using (var connection = CreateConnection())
            {
                var sql = @"
                    INSERT INTO Users (Id, Username, PasswordHash, PasswordSalt, Role, IsBlocked, FailedAttempts, LastActivityUtc, CreatedAtUtc)
                    VALUES (@Id, @Username, @PasswordHash, @PasswordSalt, @Role, @IsBlocked, @FailedAttempts, @LastActivityUtc, @CreatedAtUtc)";
                await connection.ExecuteAsync(sql, user);
            }
        }

        public async Task UpdateAsync(User user)
        {
            using (var connection = CreateConnection())
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
                await connection.ExecuteAsync(sql, user);
            }
        }
    }
}