using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Aplicacion.DTOs.Servicios
{
    /// <summary>
    /// DTO para filtrar servicios (CU-006, CU-009)
    /// </summary>
    public class FiltroServiciosRequest
    {
        // Búsqueda por texto (nombre difunto, expediente, etc.)
        public string Buscar { get; set; }

        // Filtro por estado
        public string Estado { get; set; }

        // Filtro por rango de fechas
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        // Filtro por paquete
        public string Paquete { get; set; }

        // Paginación
        public int Pagina { get; set; } = 1;
        public int TamañoPagina { get; set; } = 10;
    }
}

