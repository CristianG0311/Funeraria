using Microsoft.AspNetCore.Mvc;
using Funeraria.Aplicacion.Interfaces;
using Funeraria.Aplicacion.DTOs.Dashboard;

namespace Funeraria.Api.Controllers
{
    /// <summary>
    /// Controlador para el dashboard
    /// Expone los endpoints para CU-023 a CU-025
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// CU-023: Obtener resumen del dashboard
        /// </summary>
        [HttpGet("resumen")]
        public async Task<ActionResult<DashboardDto>> ObtenerResumen()
        {
            try
            {
                var dashboard = await _dashboardService.ObtenerResumenAsync();
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// CU-024: Generar reporte de servicios mensuales
        /// </summary>
        [HttpGet("reportes/servicios-mensuales")]
        public async Task<ActionResult> GenerarReporteServiciosMensuales(
            [FromQuery] int mes,
            [FromQuery] int año)
        {
            try
            {
                var reporte = await _dashboardService.GenerarReporteServiciosMensualesAsync(mes, año);
                return File(reporte, "application/pdf", $"Reporte-Servicios-{mes}-{año}.pdf");
            }
            catch (NotImplementedException)
            {
                return StatusCode(501, new { mensaje = "Esta funcionalidad se implementará próximamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// CU-025: Consultar estadísticas
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<object>> ConsultarEstadisticas(
            [FromQuery] DateTime fechaDesde,
            [FromQuery] DateTime fechaHasta)
        {
            try
            {
                var estadisticas = await _dashboardService.ConsultarEstadisticasAsync(fechaDesde, fechaHasta);
                return Ok(estadisticas);
            }
            catch (NotImplementedException)
            {
                return StatusCode(501, new { mensaje = "Esta funcionalidad se implementará próximamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }
}
