using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace IdentityService.Application.UseCases.Users.Create
{
    /**
     * Validador de FluentValidation para el CreateUserCommand.
     */
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("El nombre de usuario es requerido.")
                .MinimumLength(3).WithMessage("El nombre de usuario debe tener al menos 3 caracteres.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("El rol es requerido.")
                .Must(role => role.ToUpper() == "ADMIN" || role.ToUpper() == "VOTANTE")
                .WithMessage("El rol debe ser 'ADMIN' o 'VOTANTE'.");
        }
    }
}