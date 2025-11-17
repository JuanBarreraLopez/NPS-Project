using System;
using System.Collections.Generic;
using System.Text;

namespace NpsService.Application.Interfaces
{
    /*
     * Este servicio leerá el Token JWT que pasa por el ApiGateway
     * para decirle a nuestra lógica QUIÉN está haciendo la solicitud.
     */
    public interface ICurrentUserContext
    {
        // Obtiene el ID del usuario (el 'sub' claim del JWT)
        Guid GetCurrentUserId();

        // Obtiene el Rol del usuario (el 'role' claim del JWT)
        string GetCurrentUserRole();
    }
}