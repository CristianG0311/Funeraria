using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Aplicacion.DTOs.Servicios
{
    /// <summary>
    /// DTO que representa un servicio funerario completo para mostrar en el frontend
    /// </summary>
    public class ServicioFunerarioDto
    {
        public int Id { get; set; }
        public string CodigoExpediente { get; set; }

        // Datos del difunto
        public string NombreDifunto { get; set; }
        public string CedulaDifunto { get; set; }
        public DateTime FechaFallecimiento { get; set; }
        public int Edad { get; set; }
        public string LugarFallecimiento { get; set; }

        // Datos del familiar
        public string NombreFamiliar { get; set; }
        public string Parentesco { get; set; }
        public string TelefonoFamiliar { get; set; }
        public string EmailFamiliar { get; set; }

        // Detalles del servicio
        public string Paquete { get; set; }
        public string TipoServicio { get; set; }
        public string SalaVelacion { get; set; }
        public bool CeremoniaReligiosa { get; set; }
        public bool GestionDocumentalCompleta { get; set; }

        // Estado y finanzas
        public string Estado { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Observaciones { get; set; }

        // Información adicional calculada
        public int DocumentosPendientes { get; set; }
        public int DocumentosCompletos { get; set; }
        public int TotalDocumentos { get; set; }
    }
}
