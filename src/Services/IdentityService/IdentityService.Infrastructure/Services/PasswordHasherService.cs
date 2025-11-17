using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityService.Application.Interfaces;

namespace IdentityService.Infrastructure.Services
{
    /**
     * Implementación del hasher de contraseñas.
     * Utiliza PBKDF2.
     */
    public class PasswordHasherService : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32;  // 256 bit
        private const int Iterations = 10000; // Número de iteraciones

        public (byte[] PasswordHash, byte[] PasswordSalt) Hash(string password)
        {
            using (var algorithm = new Rfc2898DeriveBytes(
                password,
                SaltSize,
                Iterations,
                HashAlgorithmName.SHA256))
            {
                var salt = algorithm.Salt;
                var hash = algorithm.GetBytes(KeySize);
                return (hash, salt);
            }
        }

        public bool Verify(string password, byte[] hash, byte[] salt)
        {
            using (var algorithm = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256))
            {
                var newHash = algorithm.GetBytes(KeySize);
                return newHash.SequenceEqual(hash); // Compara los dos hashes
            }
        }
    }
}
