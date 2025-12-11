using Microsoft.AspNetCore.Mvc;
using Funeraria.Aplicacion.Interfaces;
using Funeraria.Aplicacion.DTOs.Inventario;

namespace Funeraria.Api.Controllers
{
    /// <summary>
    /// Controlador para gestión de inventario
    /// Expone los endpoints para CU-014 a CU-018
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioService _inventarioService;

        public InventarioController(IInventarioService inventarioService)
        {
            _inventarioService = inventarioService;
        }

        /// <summary>
        /// CU-014: Consultar inventario con filtros
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ArticuloInventarioDto>>> ConsultarInventario(
            [FromQuery] string? categoria,
            [FromQuery] string? estadoStock,
            [FromQuery] string? buscar)
        {
            try
            {
                var articulos = await _inventarioService.ConsultarInventarioAsync(categoria, estadoStock, buscar);
                return Ok(articulos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Crear nuevo artículo en inventario
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ArticuloInventarioDto>> CrearArticulo([FromBody] CrearArticuloRequest request)
        {
            try
            {
                var articulo = await _inventarioService.CrearArticuloAsync(request);
                return Ok(articulo);
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
        /// CU-015: Registrar entrada de inventario
        /// </summary>
        [HttpPost("entrada")]
        public async Task<ActionResult> RegistrarEntrada([FromBody] MovimientoInventarioRequest request)
        {
            try
            {
                await _inventarioService.RegistrarEntradaAsync(request);
                return Ok(new { mensaje = "Entrada registrada correctamente" });
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
        /// CU-016: Registrar salida de inventario
        /// </summary>
        [HttpPost("salida")]
        public async Task<ActionResult> RegistrarSalida([FromBody] MovimientoInventarioRequest request)
        {
            try
            {
                await _inventarioService.RegistrarSalidaAsync(request);
                return Ok(new { mensaje = "Salida registrada correctamente" });
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
        /// CU-017: Generar alertas de stock bajo
        /// </summary>
        [HttpGet("alertas")]
        public async Task<ActionResult<List<ArticuloInventarioDto>>> GenerarAlertas()
        {
            try
            {
                var articulos = await _inventarioService.GenerarAlertasStockBajoAsync();
                return Ok(articulos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// CU-018: Actualizar artículo de inventario
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarArticulo(int id, [FromBody] ArticuloInventarioDto articulo)
        {
            try
            {
                await _inventarioService.ActualizarArticuloAsync(id, articulo);
                return Ok(new { mensaje = "Artículo actualizado correctamente" });
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
    }
}
