using System;
using System.Collections.Generic;
using System.Text;
using NpsService.Domain;

namespace NpsService.Application.Interfaces
{
    /**
     * Define el contrato para el repositorio de Votos.
     * La 'Infrastructure' lo implementará con Dapper.
     */
    public interface IVoteRepository
    {
        // Para guardar el voto
        Task AddAsync(Vote vote);

        // Para el dashboard: ¿Este usuario ya votó?
        Task<bool> HasUserVotedAsync(Guid userId);

        // Para el dashboard: Traer todos los votos para calcular el NPS
        Task<IEnumerable<Vote>> GetAllAsync();
    }
}