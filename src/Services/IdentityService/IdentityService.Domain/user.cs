using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Role { get; set; } // "ADMIN" o "VOTANTE"
        public bool IsBlocked { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime LastActivityUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
