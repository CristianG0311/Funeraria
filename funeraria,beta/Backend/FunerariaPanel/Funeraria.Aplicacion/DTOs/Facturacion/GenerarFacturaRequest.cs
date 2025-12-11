using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Aplicacion.DTOs.Facturacion
{
    /// <summary>
    /// DTO para generar una factura (CU-019)
    /// </summary>
    public class GenerarFacturaRequest
    {
        public int ServicioId { get; set; }
        public decimal? Descuento { get; set; } // Opcional
        public string MetodoPago { get; set; }
        public string Observaciones { get; set; }
    }
}
