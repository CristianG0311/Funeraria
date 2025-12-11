using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Facturacion
{
    /// <summary>
    /// Clase para validaciones relacionadas con facturación
    /// Implementa manejo de excepciones según requisitos del proyecto
    /// </summary>
    public static class ValidadorFacturacion
    {
        /// <summary>
        /// Valida que un monto sea válido
        /// </summary>
        public static void ValidarMonto(decimal monto, string nombreMonto)
        {
            if (monto < 0)
            {
                throw new ArgumentException($"{nombreMonto} no puede ser negativo", nombreMonto);
            }
        }

        /// <summary>
        /// Valida que un porcentaje esté en el rango válido
        /// </summary>
        public static void ValidarPorcentaje(decimal porcentaje, string nombrePorcentaje)
        {
            if (porcentaje < 0 || porcentaje > 100)
            {
                throw new ArgumentException(
                    $"{nombrePorcentaje} debe estar entre 0 y 100", nombrePorcentaje);
            }
        }

        /// <summary>
        /// Valida que un paquete sea válido
        /// </summary>
        public static void ValidarPaquete(string paquete)
        {
            var paquetesValidos = new[] { "basico", "premium", "deluxe" };

            if (string.IsNullOrWhiteSpace(paquete))
            {
                throw new ArgumentException("El paquete no puede estar vacío", nameof(paquete));
            }

            if (!paquetesValidos.Contains(paquete.ToLower()))
            {
                throw new ArgumentException(
                    $"Paquete inválido. Los paquetes válidos son: {string.Join(", ", paquetesValidos)}",
                    nameof(paquete));
            }
        }
    }
}
