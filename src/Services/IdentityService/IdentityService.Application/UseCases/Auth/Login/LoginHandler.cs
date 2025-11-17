using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;
using System.Threading.Tasks;
using IdentityService.Domain;
using IdentityService.Application.Interfaces;
using System.Security.Authentication;

namespace IdentityService.Application.UseCases.Auth.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public LoginHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        // 2. Este es el método que MediatR ejecutará
        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {

            // 3. Buscar al usuario
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            if (user == null)
            {
                throw new AuthenticationException("Usuario o contraseña incorrectos.");
            }

            if (user.IsBlocked)
            {
                throw new AuthenticationException("La cuenta está bloqueada.");
            }

            // 5. Verificar la contraseña
            var isValidPassword = _passwordHasher.Verify(request.Password, user.PasswordHash, user.PasswordSalt);

            if (!isValidPassword)
            {
                user.FailedAttempts++;
                if (user.FailedAttempts >= 3)
                {
                    user.IsBlocked = true;
                }
                await _userRepository.UpdateAsync(user); 

                throw new AuthenticationException("Usuario o contraseña incorrectos.");
            }

            user.FailedAttempts = 0;
            user.LastActivityUtc = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user.Id);

            await _jwtService.AddRefreshTokenAsync(refreshToken);

            return new LoginResult
            {
                Username = user.Username,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
