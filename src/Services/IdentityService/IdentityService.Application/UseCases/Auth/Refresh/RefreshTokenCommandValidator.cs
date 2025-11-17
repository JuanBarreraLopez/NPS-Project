using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace IdentityService.Application.UseCases.Auth.Refresh
{
    /**
     * Validador de FluentValidation para el RefreshTokenCommand.
     */
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("El token de refresco es requerido.");
        }
    }
}