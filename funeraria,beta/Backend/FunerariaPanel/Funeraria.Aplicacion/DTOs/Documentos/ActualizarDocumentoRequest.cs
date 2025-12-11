using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Aplicacion.DTOs.Documentos
{
    /// <summary>
    /// DTO para actualizar el estado de un documento (CU-011)
    /// </summary>
    public class ActualizarDocumentoRequest
    {
        public string NumeroDocumento { get; set; }
        public DateTime? FechaEmision { get; set; }
        public string EntidadEmisora { get; set; }
        public string Estado { get; set; } // "Pendiente", "EnTramite", "Completo", "Rechazado"
        public string Observaciones { get; set; }
    }
}
