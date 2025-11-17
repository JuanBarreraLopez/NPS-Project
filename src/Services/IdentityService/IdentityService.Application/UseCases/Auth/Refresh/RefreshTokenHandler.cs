using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityService.Application.Interfaces;
using IdentityService.Application.UseCases.Auth.Login;
using MediatR;
using System.Security.Authentication;

namespace IdentityService.Application.UseCases.Auth.Refresh
{
    /**
     * Manejador de MediatR para el RefreshTokenCommand.
     * Contiene la lógica de negocio para refrescar un token.
     */
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResult>
    {
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;

        public RefreshTokenHandler(IJwtService jwtService, IUserRepository userRepository)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
        }

        public async Task<LoginResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // 1. Validar el token de refresco antiguo
            var oldToken = await _jwtService.GetRefreshTokenAsync(request.RefreshToken);

            if (oldToken == null)
            {
                throw new AuthenticationException("Token de refresco inválido o expirado.");
            }

            // 3. "Quemamos" el token antiguo para que no se pueda re-usar
            oldToken.Revoked = true;
            await _jwtService.UpdateRefreshTokenAsync(oldToken);

            // 4. Buscamos al usuario dueño del token
            var user = await _userRepository.GetByIdAsync(oldToken.UserId);
            if (user == null || user.IsBlocked)
            {
                throw new AuthenticationException("Usuario no encontrado o bloqueado.");
            }

            // 5. Generamos un *nuevo* par de tokens
            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken(user.Id);

            // 6. Guardamos el *nuevo* refresh token en la BBDD
            await _jwtService.AddRefreshTokenAsync(newRefreshToken);

            // 7. Devolvemos el resultado
            return new LoginResult
            {
                Username = user.Username,
                Role = user.Role,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }
    }
}