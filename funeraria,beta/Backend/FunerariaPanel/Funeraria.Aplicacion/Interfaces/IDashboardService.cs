using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Funeraria.Aplicacion.DTOs.Dashboard;

namespace Funeraria.Aplicacion.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio del dashboard
    /// Implementa los casos de uso CU-023 a CU-025
    /// </summary>
    public interface IDashboardService
    {
        // CU-023: Ver dashboard
        Task<DashboardDto> ObtenerResumenAsync();

        // CU-024: Generar reporte de servicios mensuales
        Task<byte[]> GenerarReporteServiciosMensualesAsync(int mes, int año);

        // CU-025: Consultar estadísticas
        Task<object> ConsultarEstadisticasAsync(DateTime fechaDesde, DateTime fechaHasta);
    }
}

