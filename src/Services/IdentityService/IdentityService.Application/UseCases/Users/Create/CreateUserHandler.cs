using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityService.Application.Interfaces;
using IdentityService.Domain;
using MediatR;
using FluentValidation;

namespace IdentityService.Application.UseCases.Users.Create
{
    /**
     * Manejador de MediatR para el CreateUserCommand.
     * Contiene la lógica para crear un nuevo usuario.
     */
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                throw new ValidationException($"El nombre de usuario '{request.Username}' ya está en uso.");
            }

            var (hash, salt) = _passwordHasher.Hash(request.Password);

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = request.Role.ToUpper(),
                IsBlocked = false,
                FailedAttempts = 0,
                LastActivityUtc = DateTime.UtcNow,
                CreatedAtUtc = DateTime.UtcNow
            };


            await _userRepository.AddAsync(newUser);

            return newUser.Id;
        }
    }
}