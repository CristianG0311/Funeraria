using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Dominio.Entidades
{
    /// <summary>
    /// Representa un servicio funerario completo (CU-005 a CU-009)
    /// </summary>
    public class ServicioFunerario
    {
        // Identificador único del servicio
        public int Id { get; set; }

        // Código de expediente (ejemplo: EXP-2025-001)
        public string CodigoExpediente { get; set; }

        // ===== DATOS DEL DIFUNTO =====
        public string NombreDifunto { get; set; }
        public string CedulaDifunto { get; set; }
        public DateTime FechaFallecimiento { get; set; }
        public int Edad { get; set; }
        public string LugarFallecimiento { get; set; }

        // ===== DATOS DEL FAMILIAR RESPONSABLE =====
        public string NombreFamiliar { get; set; }
        public string Parentesco { get; set; }
        public string TelefonoFamiliar { get; set; }
        public string EmailFamiliar { get; set; }

        // ===== DETALLES DEL SERVICIO =====
        // Paquete: "basico", "premium", "deluxe"
        public string Paquete { get; set; }

        // Tipo de servicio: "entierro", "cremacion", "nicho"
        public string TipoServicio { get; set; }

        // Sala: "sala1", "sala2", "sala3", "sin_sala"
        public string SalaVelacion { get; set; }

        public bool CeremoniaReligiosa { get; set; }
        public bool GestionDocumentalCompleta { get; set; }

        // ===== ESTADO Y FINANZAS =====
        // Estado: "Registrado", "EnProceso", "Completado", "Cancelado"
        public string Estado { get; set; }

        public decimal Total { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Observaciones adicionales del servicio
        public string Observaciones { get; set; }
    }
}

