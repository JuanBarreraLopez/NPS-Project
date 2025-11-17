using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace NpsService.Application.UseCases.Votes.Submit
{
    public class SubmitVoteCommandValidator : AbstractValidator<SubmitVoteCommand>
    {
        public SubmitVoteCommandValidator()
        {
            RuleFor(x => x.Score)
                .InclusiveBetween(0, 10).WithMessage("El puntaje debe estar entre 0 y 10.");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("El comentario no puede exceder los 500 caracteres.");
        }
    }
}