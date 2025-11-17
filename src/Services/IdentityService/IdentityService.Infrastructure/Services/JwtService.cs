using IdentityService.Application.Interfaces;
using IdentityService.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper; 

namespace IdentityService.Infrastructure.Services
{
    /**
     * Implementación del servicio de JWT 
     * Firma tokens y maneja refresh tokens en la BBDD
     */
    public class JwtService : IJwtService
    {
        private readonly string _connectionString;
        private readonly IConfiguration _config;

        // Inyectamos la configuración para leer los secretos y la BBDD
        public JwtService(IConfiguration config)
        {
            _config = config;
            // Esto lo configuraremos globalmente después.
            _connectionString = _config.GetConnectionString("IdentityDb") ?? "Server=...;Database=IdentityDB;...;";
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // --- 1. Generación de Access Token ---
        public string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"] ?? "UNA_CLAVE_SECRETA_MUY_LARGA_PARA_PRUEBAS"); 

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role) // ¡Importante para los permisos!
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                // El token de acceso dura poco
                // 5 minutos de inactividad, pero el token puede durar más
                // La *sesión* se manejará en la capa Application.
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // --- 2. Generación de Refresh Token ---
        public RefreshToken GenerateRefreshToken(Guid userId)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var randomBytes = new byte[64];
                rng.GetBytes(randomBytes);
                var token = Convert.ToBase64String(randomBytes);

                return new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Token = token,
                    CreatedAtUtc = DateTime.UtcNow,
                    // Los refresh tokens duran más
                    ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
                    Revoked = false
                };
            }
        }

        // --- 3. Métodos de Dapper para Refresh Tokens ---
        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            using (var connection = CreateConnection())
            {
                var sql = @"
                    INSERT INTO RefreshTokens (Id, UserId, Token, ExpiresAtUtc, CreatedAtUtc, Revoked)
                    VALUES (@Id, @UserId, @Token, @ExpiresAtUtc, @CreatedAtUtc, @Revoked)";
                await connection.ExecuteAsync(sql, refreshToken);
            }
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            using (var connection = CreateConnection())
            {
                var sql = "SELECT * FROM RefreshTokens WHERE Token = @Token AND Revoked = 0 AND ExpiresAtUtc > @Now";
                return await connection.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { Token = token, Now = DateTime.UtcNow });
            }
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken token)
        {
            using (var connection = CreateConnection())
            {
                var sql = "UPDATE RefreshTokens SET Revoked = @Revoked WHERE Id = @Id";
                await connection.ExecuteAsync(sql, token);
            }
        }
    }
}