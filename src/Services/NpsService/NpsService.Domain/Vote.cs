using System;
using System.Collections.Generic;
using System.Text;

namespace NpsService.Domain
{
    public class Vote
    {
        public Guid Id { get; set; }

        // El ID del usuario que votó (viene del JWT Token)
        public Guid UserId { get; set; }

        // El puntaje NPS (0-10)
        public int Score { get; set; }

        // El comentario opcional
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }
    }
}