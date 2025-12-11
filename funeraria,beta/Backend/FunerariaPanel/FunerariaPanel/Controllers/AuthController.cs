using Microsoft.AspNetCore.Mvc;
using Funeraria.Aplicacion.Interfaces;
using Funeraria.Aplicacion.DTOs.Auth;

namespace Funeraria.Api.Controllers
{
    /// <summary>
    /// Controlador para autenticación
    /// Expone los endpoints para CU-001
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// CU-001: Iniciar sesión en el sistema
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.IniciarSesionAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensaje = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Validar token
        /// </summary>
        [HttpPost("validar-token")]
        public async Task<ActionResult<bool>> ValidarToken([FromBody] string token)
        {
            try
            {
                var esValido = await _authService.ValidarTokenAsync(token);
                return Ok(new { valido = esValido });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }
}
