using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Aplicacion.DTOs.Facturacion
{
    /// <summary>
    /// DTO para registrar un pago (CU-021)
    /// </summary>
    public class RegistrarPagoRequest
    {
        public int FacturaId { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; }
        public string NumeroReferencia { get; set; }
        public string Observaciones { get; set; }
    }
}
