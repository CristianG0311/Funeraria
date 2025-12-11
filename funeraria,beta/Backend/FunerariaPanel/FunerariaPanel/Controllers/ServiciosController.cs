using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Funeraria.Aplicacion.Interfaces;
using Funeraria.Aplicacion.DTOs.Servicios;

namespace Funeraria.Api.Controllers
{
    /// <summary>
    /// Controlador para gestión de servicios funerarios
    /// Expone los endpoints para CU-005 a CU-009
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ServiciosController : ControllerBase
    {
        private readonly IServicioFunerarioService _servicioService;

        public ServiciosController(IServicioFunerarioService servicioService)
        {
            _servicioService = servicioService;
        }

        /// <summary>
        /// CU-005: Registrar un nuevo servicio funerario
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ServicioFunerarioDto>> RegistrarServicio(
            [FromBody] RegistrarServicioRequest request)
        {
            try
            {
                var resultado = await _servicioService.RegistrarServicioAsync(request);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
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
        /// CU-006: Listar servicios con filtros
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ServicioFunerarioDto>>> ListarServicios(
            [FromQuery] string? buscar,
            [FromQuery] string? estado,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta,
            [FromQuery] string? paquete)
        {
            try
            {
                var filtro = new FiltroServiciosRequest
                {
                    Buscar = buscar,
                    Estado = estado,
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta,
                    Paquete = paquete
                };

                var servicios = await _servicioService.ListarServiciosAsync(filtro);
                return Ok(servicios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtener servicio por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ServicioFunerarioDto>> ObtenerPorId(int id)
        {
            try
            {
                var servicio = await _servicioService.ObtenerPorIdAsync(id);
                return Ok(servicio);
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// CU-009: Buscar servicio por código de expediente
        /// </summary>
        [HttpGet("expediente/{codigoExpediente}")]
        public async Task<ActionResult<ServicioFunerarioDto>> BuscarPorExpediente(string codigoExpediente)
        {
            try
            {
                var servicio = await _servicioService.BuscarPorExpedienteAsync(codigoExpediente);
                return Ok(servicio);
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// CU-007: Cambiar estado del servicio
        /// </summary>
        [HttpPut("{id}/estado")]
        public async Task<ActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoServicioRequest request)
        {
            try
            {
                await _servicioService.CambiarEstadoAsync(id, request);
                return Ok(new { mensaje = "Estado actualizado correctamente" });
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
        /// CU-008: Cancelar servicio
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> CancelarServicio(int id)
        {
            try
            {
                await _servicioService.CancelarServicioAsync(id);
                return Ok(new { mensaje = "Servicio cancelado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }
}

