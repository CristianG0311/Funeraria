using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Facturacion
{
    /// <summary>
    /// Modelo que contiene el resultado completo del cálculo de una factura
    /// </summary>
    public class ResultadoFactura
    {
        // Lista de conceptos facturados
        public List<DetalleFactura> Detalles { get; set; }

        // Subtotal antes de descuentos e impuestos
        public decimal Subtotal { get; set; }

        // Descuento aplicado
        public decimal Descuento { get; set; }

        // Subtotal después del descuento
        public decimal SubtotalConDescuento { get; set; }

        // ITBMS calculado (7%)
        public decimal Itbms { get; set; }

        // Total final
        public decimal Total { get; set; }

        public ResultadoFactura()
        {
            Detalles = new List<DetalleFactura>();
        }
    }
}
