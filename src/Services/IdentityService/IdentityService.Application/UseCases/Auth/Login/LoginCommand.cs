using System;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.UseCases.Auth.Login
{
    public class LoginCommand : IRequest<LoginResult>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
