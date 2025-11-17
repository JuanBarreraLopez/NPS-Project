using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using NpsService.Application.Interfaces;
using System.Security.Claims;

namespace NpsService.Infrastructure.Services
{
    /**
     * Implementación del lector de contexto de usuario.
     * Lee el token JWT que viene en la solicitud HTTP.
     */
    public class CurrentUserContext : ICurrentUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

        public Guid GetCurrentUserId()
        {
            var userIdClaim = User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new ApplicationException("No se pudo encontrar el ID del usuario en el token JWT.");
            }

            return Guid.Parse(userIdClaim);
        }

        public string GetCurrentUserRole()
        {
            var roleClaim = User?.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(roleClaim))
            {
                throw new ApplicationException("No se pudo encontrar el Rol del usuario en el token JWT.");
            }

            return roleClaim;
        }
    }
}