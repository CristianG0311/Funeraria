using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Aplicacion.DTOs.Servicios
{
    /// <summary>
    /// DTO para cambiar el estado de un servicio (CU-007)
    /// </summary>
    public class CambiarEstadoServicioRequest
    {
        public string NuevoEstado { get; set; } // "Registrado", "EnProceso", "Completado", "Cancelado"
        public string Observaciones { get; set; }
    }
}
