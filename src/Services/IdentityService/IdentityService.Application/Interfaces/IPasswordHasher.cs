using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Interfaces
{
    public interface IPasswordHasher
    {
        (byte[] PasswordHash, byte[] PasswordSalt) Hash(string password);
        bool Verify(string password, byte[] hash, byte[] salt);
    }
}
