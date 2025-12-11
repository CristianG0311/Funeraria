using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Dominio.Entidades
{
    /// <summary>
    /// Representa un pago realizado a una factura (CU-021)
    /// </summary>
    public class Pago
    {
        // Identificador único del pago
        public int Id { get; set; }

        // ID de la factura a la que se aplica el pago
        public int FacturaId { get; set; }

        // Monto del pago
        public decimal Monto { get; set; }

        // Fecha del pago
        public DateTime FechaPago { get; set; }

        // Método de pago: "Efectivo", "Tarjeta", "Transferencia"
        public string MetodoPago { get; set; }

        // Número de referencia (para transferencias o tarjetas)
        public string NumeroReferencia { get; set; }

        // Usuario que registró el pago
        public int UsuarioId { get; set; }

        // Observaciones del pago
        public string Observaciones { get; set; }
    }
}
