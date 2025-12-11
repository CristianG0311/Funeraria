using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Aplicacion.DTOs.Facturacion
{
    /// <summary>
    /// DTO para facturas (CU-019 a CU-022)
    /// </summary>
    public class FacturaDto
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public string CodigoExpediente { get; set; }
        public string NombreDifunto { get; set; }
        public string NumeroFactura { get; set; }
        public DateTime FechaEmision { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Itbms { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }

        public bool Pagada { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal SaldoPendiente { get; set; }

        public string MetodoPago { get; set; }
        public string RutaPdf { get; set; }
        public string Observaciones { get; set; }
    }
}
