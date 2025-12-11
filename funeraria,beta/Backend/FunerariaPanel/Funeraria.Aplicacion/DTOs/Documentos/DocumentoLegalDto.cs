using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Aplicacion.DTOs.Documentos
{
    /// <summary>
    /// DTO para documentos legales (CU-010 a CU-013)
    /// </summary>
    public class DocumentoLegalDto
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public string CodigoExpediente { get; set; } // Para mostrar en la lista
        public string NombreDifunto { get; set; } // Para mostrar en la lista

        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime? FechaEmision { get; set; }
        public string EntidadEmisora { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public string RutaArchivo { get; set; }
        public bool EsObligatorio { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    /// <summary>
    /// DTO para crear documentos legales (CU-010)
    /// </summary>
    public class CrearDocumentoRequest
    {
        public int ServicioId { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string EntidadEmisora { get; set; }
        public DateTime? FechaEmision { get; set; }
        public string Estado { get; set; }
        public bool EsObligatorio { get; set; }
        public string Observaciones { get; set; }
    }



}
