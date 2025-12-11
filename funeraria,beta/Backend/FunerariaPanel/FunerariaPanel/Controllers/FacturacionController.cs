using Microsoft.AspNetCore.Mvc;
using Funeraria.Aplicacion.Interfaces;
using Funeraria.Aplicacion.DTOs.Facturacion;

namespace Funeraria.Api.Controllers
{
    /// <summary>
    /// Controlador para gestión de facturación
    /// Expone los endpoints para CU-019 a CU-022
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FacturacionController : ControllerBase
    {
        private readonly IFacturacionService _facturacionService;

        public FacturacionController(IFacturacionService facturacionService)
        {
            _facturacionService = facturacionService;
        }

        /// <summary>
        /// CU-019: Generar factura para un servicio
        /// </summary>
        [HttpPost("generar")]
        public async Task<ActionResult<FacturaDto>> GenerarFactura([FromBody] GenerarFacturaRequest request)
        {
            try
            {
                var factura = await _facturacionService.GenerarFacturaAsync(request);
                return Ok(factura);
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
        /// CU-020: Consultar facturas con filtros
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<FacturaDto>>> ConsultarFacturas(
            [FromQuery] bool? pagadas,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta)
        {
            try
            {
                var facturas = await _facturacionService.ConsultarFacturasAsync(pagadas, fechaDesde, fechaHasta);
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtener factura por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<FacturaDto>> ObtenerPorId(int id)
        {
            try
            {
                var factura = await _facturacionService.ObtenerPorIdAsync(id);
                return Ok(factura);
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// CU-021: Registrar pago de una factura
        /// </summary>
        [HttpPost("pagos")]
        public async Task<ActionResult> RegistrarPago([FromBody] RegistrarPagoRequest request)
        {
            try
            {
                await _facturacionService.RegistrarPagoAsync(request);
                return Ok(new { mensaje = "Pago registrado correctamente" });
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

