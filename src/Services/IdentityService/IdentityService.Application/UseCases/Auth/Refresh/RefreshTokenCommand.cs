using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityService.Application.UseCases.Auth.Login;
using MediatR;

namespace IdentityService.Application.UseCases.Auth.Refresh
{
    public class RefreshTokenCommand : IRequest<LoginResult>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}