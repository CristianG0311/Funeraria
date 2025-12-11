using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Dominio.Entidades
{
    /// <summary>
    /// Representa una factura generada para un servicio (CU-019 a CU-022)
    /// </summary>
    public class Factura
    {
        // Identificador único de la factura
        public int Id { get; set; }

        // ID del servicio facturado
        public int ServicioId { get; set; }

        // Número de factura único (ejemplo: "FAC-2025-001")
        public string NumeroFactura { get; set; }

        // Fecha y hora de emisión de la factura
        public DateTime FechaEmision { get; set; }

        // Subtotal antes de impuestos
        public decimal Subtotal { get; set; }

        // ITBMS (7% según RN-017)
        public decimal Itbms { get; set; }

        // Descuento aplicado (si existe)
        public decimal Descuento { get; set; }

        // Total final de la factura
        public decimal Total { get; set; }

        // Indica si la factura está completamente pagada
        public bool Pagada { get; set; }

        // Monto pagado hasta el momento
        public decimal MontoPagado { get; set; }

        // Saldo pendiente
        public decimal SaldoPendiente { get; set; }

        // Método de pago: "Efectivo", "Tarjeta", "Transferencia", "Mixto"
        public string MetodoPago { get; set; }

        // Ruta del archivo PDF de la factura
        public string RutaPdf { get; set; }

        // Observaciones adicionales
        public string Observaciones { get; set; }
    }
}
