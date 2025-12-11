using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Aplicacion.DTOs.Servicios
{
    /// <summary>
    /// DTO para registrar un nuevo servicio funerario (CU-005)
    /// </summary>
    public class RegistrarServicioRequest
    {
        // ===== DATOS DEL DIFUNTO =====
        public string NombreDifunto { get; set; }
        public string CedulaDifunto { get; set; }
        public DateTime FechaFallecimiento { get; set; }
        public int Edad { get; set; }
        public string LugarFallecimiento { get; set; }

        // ===== DATOS DEL FAMILIAR =====
        public string NombreFamiliar { get; set; }
        public string Parentesco { get; set; }
        public string TelefonoFamiliar { get; set; }
        public string EmailFamiliar { get; set; }

        // ===== DETALLES DEL SERVICIO =====
        public string Paquete { get; set; } // "basico", "premium", "deluxe"
        public string TipoServicio { get; set; } // "entierro", "cremacion", "nicho"
        public string SalaVelacion { get; set; }
        public bool CeremoniaReligiosa { get; set; }
        public bool GestionDocumentalCompleta { get; set; }
        public string Observaciones { get; set; }
    }
}
