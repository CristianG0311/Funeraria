using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Aplicacion.DTOs.Dashboard
{
    /// <summary>
    /// DTO para el dashboard principal (CU-023)
    /// </summary>
    public class DashboardDto
    {
        // Contadores principales
        public int ServiciosActivos { get; set; }
        public int DocumentosPendientes { get; set; }
        public int TrasladosHoy { get; set; }
        public decimal FacturacionMes { get; set; }

        // Inventario crítico
        public int ArticulosCriticos { get; set; }

        // Servicios recientes
        public List<ServicioResumenDto> ServiciosRecientes { get; set; }

        // Alertas
        public List<AlertaDto> Alertas { get; set; }
    }

    public class ServicioResumenDto
    {
        public int Id { get; set; }
        public string CodigoExpediente { get; set; }
        public string NombreDifunto { get; set; }
        public DateTime FechaFallecimiento { get; set; }
        public string Estado { get; set; }
        public decimal Total { get; set; }
    }

    public class AlertaDto
    {
        public string Tipo { get; set; } // "Documento", "Inventario", "Pago"
        public string Mensaje { get; set; }
        public string Nivel { get; set; } // "Info", "Advertencia", "Critico"
        public DateTime Fecha { get; set; }
    }
}
