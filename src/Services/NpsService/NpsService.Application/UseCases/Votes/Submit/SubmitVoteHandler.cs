using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using MediatR;
using NpsService.Application.Interfaces;
using NpsService.Domain;

namespace NpsService.Application.UseCases.Votes.Submit
{
    /**
     * Manejador de MediatR para el SubmitVoteCommand.
     * Contiene la lógica para guardar un nuevo voto.
     */
    public class SubmitVoteHandler : IRequestHandler<SubmitVoteCommand>
    {
        private readonly IVoteRepository _voteRepository;
        private readonly ICurrentUserContext _currentUserContext;

        public SubmitVoteHandler(IVoteRepository voteRepository, ICurrentUserContext currentUserContext)
        {
            _voteRepository = voteRepository;
            _currentUserContext = currentUserContext;
        }

        public async Task Handle(SubmitVoteCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserContext.GetCurrentUserId();

            var hasVoted = await _voteRepository.HasUserVotedAsync(userId);
            if (hasVoted)
            {
                throw new ValidationException("Este usuario ya ha enviado su voto.");
            }

            // 3. Crear la entidad de Voto
            var vote = new Vote
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Score = request.Score,
                Comment = request.Comment ?? string.Empty,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _voteRepository.AddAsync(vote);
        }
    }
}