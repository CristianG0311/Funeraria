using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funeraria.Aplicacion.DTOs.Dashboard;
using Funeraria.Aplicacion.Interfaces;
using Funeraria.Dominio.Interfaces;

namespace Funeraria.Aplicacion.Servicios
{
    /// <summary>
    /// Servicio de aplicación para el dashboard
    /// Implementa los casos de uso CU-023 a CU-025
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IServicioFunerarioRepository _servicioRepository;
        private readonly IDocumentoLegalRepository _documentoRepository;
        private readonly IInventarioRepository _inventarioRepository;
        private readonly IFacturacionRepository _facturacionRepository;

        public DashboardService(
            IServicioFunerarioRepository servicioRepository,
            IDocumentoLegalRepository documentoRepository,
            IInventarioRepository inventarioRepository,
            IFacturacionRepository facturacionRepository)
        {
            _servicioRepository = servicioRepository;
            _documentoRepository = documentoRepository;
            _inventarioRepository = inventarioRepository;
            _facturacionRepository = facturacionRepository;
        }

        // CU-023: Ver dashboard
        public async Task<DashboardDto> ObtenerResumenAsync()
        {
            var dashboard = new DashboardDto
            {
                // Contadores principales
                ServiciosActivos = await _servicioRepository.ContarServiciosActivosAsync(),
                DocumentosPendientes = await _documentoRepository.ContarPendientesAsync(),
                TrasladosHoy = 0, // TODO: Implementar lógica de traslados
                FacturacionMes = await _facturacionRepository.ObtenerFacturacionMesAsync(),
                ArticulosCriticos = await _inventarioRepository.ContarArticulosCriticosAsync(),

                // Servicios recientes
                ServiciosRecientes = new List<ServicioResumenDto>(),

                // Alertas
                Alertas = new List<AlertaDto>()
            };

            // Obtener servicios recientes
            var serviciosRecientes = await _servicioRepository.ObtenerRecientesAsync(5);

            dashboard.ServiciosRecientes = serviciosRecientes.Select(s => new ServicioResumenDto
            {
                Id = s.Id,
                CodigoExpediente = s.CodigoExpediente,
                NombreDifunto = s.NombreDifunto,
                FechaFallecimiento = s.FechaFallecimiento,
                Estado = s.Estado,
                Total = s.Total
            }).ToList();

            // Generar alertas
            await GenerarAlertasAsync(dashboard);

            return dashboard;
        }

        // CU-024: Generar reporte de servicios mensuales
        public async Task<byte[]> GenerarReporteServiciosMensualesAsync(int mes, int año)
        {
            // Este método generaría un PDF o Excel con el reporte
            throw new NotImplementedException("La generación de reportes se implementará en una fase posterior");
        }

        // CU-025: Consultar estadísticas
        public async Task<object> ConsultarEstadisticasAsync(DateTime fechaDesde, DateTime fechaHasta)
        {
            // Aquí se calcularían estadísticas personalizadas
            throw new NotImplementedException("Las estadísticas avanzadas se implementarán en una fase posterior");
        }

        // Método privado para generar alertas
        private async Task GenerarAlertasAsync(DashboardDto dashboard)
        {
            // Alertas de documentos pendientes
            if (dashboard.DocumentosPendientes > 0)
            {
                dashboard.Alertas.Add(new AlertaDto
                {
                    Tipo = "Documento",
                    Mensaje = $"Hay {dashboard.DocumentosPendientes} documento(s) pendiente(s) de completar",
                    Nivel = dashboard.DocumentosPendientes > 5 ? "Critico" : "Advertencia",
                    Fecha = DateTime.Now
                });
            }

            // Alertas de inventario crítico
            if (dashboard.ArticulosCriticos > 0)
            {
                dashboard.Alertas.Add(new AlertaDto
                {
                    Tipo = "Inventario",
                    Mensaje = $"Hay {dashboard.ArticulosCriticos} artículo(s) con stock crítico",
                    Nivel = "Critico",
                    Fecha = DateTime.Now
                });
            }

            // Alertas de facturas pendientes de pago
            var facturasPendientes = await _facturacionRepository.ConsultarFacturasAsync(pagadas: false);

            if (facturasPendientes.Any())
            {
                var totalPendiente = facturasPendientes.Sum(f => f.SaldoPendiente);

                dashboard.Alertas.Add(new AlertaDto
                {
                    Tipo = "Pago",
                    Mensaje = $"Hay {facturasPendientes.Count} factura(s) con saldo pendiente: {totalPendiente:C}",
                    Nivel = "Advertencia",
                    Fecha = DateTime.Now
                });
            }
        }
    }
}
