using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Facturacion
{
    /// <summary>
    /// Modelo que representa el detalle de una factura
    /// </summary>
    public class DetalleFactura
    {
        public string Concepto { get; set; }
        public decimal Monto { get; set; }
    }
}
