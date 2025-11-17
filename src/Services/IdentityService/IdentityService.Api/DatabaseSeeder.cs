using Dapper;
using IdentityService.Application.Interfaces;
using IdentityService.Domain;
using System.Data;

namespace IdentityService.Api
{
    /*
     * Su único trabajo es revisar la base de datos y crear el usuario admin
     * si la tabla está vacía.
     */
    public static class DatabaseSeeder
    {
        public static void SeedDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var connection = services.GetRequiredService<IDbConnection>();
                    var passwordHasher = services.GetRequiredService<IPasswordHasher>();

                    connection.Open();

                    var userCount = connection.QueryFirst<int>("SELECT COUNT(*) FROM Users");

                    if (userCount == 0)
                    {
                        Console.WriteLine("Base de datos vacía. Sembramos el usuario admin...");

                        var (hash, salt) = passwordHasher.Hash("Admin123!");

                        var adminUser = new User
                        {
                            Id = Guid.NewGuid(),
                            Username = "admin",
                            PasswordHash = hash,
                            PasswordSalt = salt,
                            Role = "ADMIN", 
                            IsBlocked = false,
                            FailedAttempts = 0,
                            LastActivityUtc = DateTime.UtcNow,
                            CreatedAtUtc = DateTime.UtcNow
                        };

                        var sql = @"
                            INSERT INTO Users (Id, Username, PasswordHash, PasswordSalt, Role, IsBlocked, FailedAttempts, LastActivityUtc, CreatedAtUtc)
                            VALUES (@Id, @Username, @PasswordHash, @PasswordSalt, @Role, @IsBlocked, @FailedAttempts, @LastActivityUtc, @CreatedAtUtc)";

                        connection.Execute(sql, adminUser);

                        Console.WriteLine("¡Usuario Admin creado!");
                        Console.WriteLine("Usuario: admin");
                        Console.WriteLine("Contra: Admin123!");
                    }
                    else
                    {
                        Console.WriteLine("La base de datos ya tiene usuarios. No se siembra nada.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocurrió un error al sembrar la base de datos: {ex.Message}");
                }
            }
        }
    }
}