using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funeraria.Aplicacion.DTOs.Auth;
using Funeraria.Aplicacion.Interfaces;
using Funeraria.Dominio.Interfaces;

namespace Funeraria.Aplicacion.Servicios
{
    /// <summary>
    /// Servicio de aplicación para autenticación
    /// Implementa el caso de uso CU-001
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // CU-001: Iniciar sesión
        public async Task<LoginResponse> IniciarSesionAsync(LoginRequest request)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(request.NombreUsuario))
            {
                throw new ArgumentException("El nombre de usuario es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("La contraseña es obligatoria");
            }

            // Buscar usuario
            var usuario = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(request.NombreUsuario);

            // Validar credenciales
            if (usuario == null || usuario.PasswordHash != request.Password)
            {
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");
            }

            // Actualizar último acceso
            await _usuarioRepository.ActualizarUltimoAccesoAsync(usuario.Id);

            // Generar respuesta
            // TODO: En producción, aquí se generaría un JWT token real
            return new LoginResponse
            {
                Token = GenerarTokenSimulado(usuario.Id),
                NombreUsuario = usuario.NombreUsuario,
                NombreCompleto = usuario.NombreCompleto,
                Rol = usuario.Rol,
                FechaExpiracion = DateTime.Now.AddHours(8)
            };
        }

        // Validar token JWT
        public async Task<bool> ValidarTokenAsync(string token)
        {
            // TODO: Implementar validación real de JWT
            // Por ahora, aceptamos cualquier token no vacío
            return !string.IsNullOrWhiteSpace(token);
        }

        // Método privado para generar token simulado
        private string GenerarTokenSimulado(int usuarioId)
        {
            // En producción, aquí se usaría JWT (System.IdentityModel.Tokens.Jwt)
            return $"TOKEN-{usuarioId}-{Guid.NewGuid()}";
        }
    }
}
