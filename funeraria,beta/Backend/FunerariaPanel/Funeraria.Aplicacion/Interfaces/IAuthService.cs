using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funeraria.Aplicacion.DTOs.Auth;

namespace Funeraria.Aplicacion.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de autenticación
    /// Implementa el caso de uso CU-001
    /// </summary>
    public interface IAuthService
    {
        // CU-001: Iniciar sesión
        Task<LoginResponse> IniciarSesionAsync(LoginRequest request);

        // Validar token JWT
        Task<bool> ValidarTokenAsync(string token);
    }
}
