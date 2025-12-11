using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funeraria.Dominio.Entidades;

namespace Funeraria.Facturacion
{
    /// <summary>
    /// Clase de ejemplo para mostrar cómo usar la DLL de facturación
    /// NO es parte del sistema, es solo para documentación
    /// </summary>
    public class EjemploUso
    {
        public static void Ejemplo()
        {
            // Crear un servicio de ejemplo
            var servicio = new ServicioFunerario
            {
                Paquete = "premium",
                CeremoniaReligiosa = true,
                GestionDocumentalCompleta = false
            };

            // Crear instancia de la calculadora
            var calculadora = new CalculadoraFactura();

            // Calcular factura sin descuento
            var resultado = calculadora.CalcularFactura(servicio);

            // El resultado contiene:
            // - resultado.Detalles: Lista de conceptos
            // - resultado.Subtotal: 2,800 (2,500 paquete + 300 ceremonia)
            // - resultado.Itbms: 196 (7% de 2,800)
            // - resultado.Total: 2,996

            // Calcular factura con descuento del 10%
            var resultadoConDescuento = calculadora.CalcularFactura(servicio,
                descuentoPorcentaje: 10m);

            // Calcular total rápido
            decimal totalRapido = calculadora.CalcularTotalRapido("premium", true, false);
        }
    }
}
