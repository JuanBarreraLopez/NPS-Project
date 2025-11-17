using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace NpsService.Application.UseCases.Votes.Submit
{
    /**
     * Comando de MediatR para enviar un nuevo voto.
     * Este comando no devuelve nada (IRequest).
     */
    public class SubmitVoteCommand : IRequest
    {
        public int Score { get; set; }
        public string? Comment { get; set; }
    }
}