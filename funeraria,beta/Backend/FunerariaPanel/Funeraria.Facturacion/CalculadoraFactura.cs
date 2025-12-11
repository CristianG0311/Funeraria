using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funeraria.Dominio.Entidades;

namespace Funeraria.Facturacion
{
    /// <summary>
    /// Clase principal para calcular facturas
    /// Implementa las reglas de negocio RN-003 a RN-007 y RN-017
    /// Esta es la DLL requerida por el proyecto
    /// </summary>
    public class CalculadoraFactura
    {
        /// <summary>
        /// Calcula la factura completa para un servicio funerario
        /// </summary>
        /// <param name="servicio">El servicio funerario a facturar</param>
        /// <param name="descuentoMonto">Descuento en monto (opcional)</param>
        /// <param name="descuentoPorcentaje">Descuento en porcentaje (opcional)</param>
        /// <returns>Resultado completo del cálculo</returns>
        public ResultadoFactura CalcularFactura(ServicioFunerario servicio,
            decimal? descuentoMonto = null, decimal? descuentoPorcentaje = null)
        {
            // Validar que el servicio no sea nulo
            if (servicio == null)
            {
                throw new ArgumentNullException(nameof(servicio), "El servicio no puede ser nulo");
            }

            var resultado = new ResultadoFactura();

            // 1. Calcular el precio base según el paquete (RN-003, RN-004, RN-005)
            decimal precioBase = ObtenerPrecioPaquete(servicio.Paquete);
            resultado.Detalles.Add(new DetalleFactura
            {
                Concepto = $"Paquete {CapitalizarPalabra(servicio.Paquete)}",
                Monto = precioBase
            });

            // 2. Agregar ceremonia religiosa si aplica (RN-006)
            if (servicio.CeremoniaReligiosa)
            {
                resultado.Detalles.Add(new DetalleFactura
                {
                    Concepto = "Ceremonia Religiosa",
                    Monto = ConstantesFacturacion.PRECIO_CEREMONIA_RELIGIOSA
                });
            }

            // 3. Agregar gestión documental si aplica
            if (servicio.GestionDocumentalCompleta)
            {
                resultado.Detalles.Add(new DetalleFactura
                {
                    Concepto = "Gestión Documental Completa",
                    Monto = ConstantesFacturacion.PRECIO_GESTION_DOCUMENTAL
                });
            }

            // 4. Calcular subtotal
            resultado.Subtotal = resultado.Detalles.Sum(d => d.Monto);

            // 5. Calcular y aplicar descuento
            resultado.Descuento = CalcularDescuento(resultado.Subtotal, descuentoMonto, descuentoPorcentaje);
            resultado.SubtotalConDescuento = resultado.Subtotal - resultado.Descuento;

            // 6. Calcular ITBMS (7%) - RN-007 y RN-017
            resultado.Itbms = Math.Round(resultado.SubtotalConDescuento * ConstantesFacturacion.PORCENTAJE_ITBMS, 2);

            // 7. Calcular total final
            resultado.Total = resultado.SubtotalConDescuento + resultado.Itbms;

            return resultado;
        }

        /// <summary>
        /// Obtiene el precio del paquete según el tipo
        /// Implementa RN-003, RN-004, RN-005
        /// </summary>
        private decimal ObtenerPrecioPaquete(string paquete)
        {
            return paquete?.ToLower() switch
            {
                "basico" => ConstantesFacturacion.PRECIO_PAQUETE_BASICO,
                "premium" => ConstantesFacturacion.PRECIO_PAQUETE_PREMIUM,
                "deluxe" => ConstantesFacturacion.PRECIO_PAQUETE_DELUXE,
                _ => throw new ArgumentException($"Paquete inválido: {paquete}. " +
                    "Los paquetes válidos son: basico, premium, deluxe", nameof(paquete))
            };
        }

        /// <summary>
        /// Calcula el descuento a aplicar
        /// </summary>
        private decimal CalcularDescuento(decimal subtotal, decimal? descuentoMonto, decimal? descuentoPorcentaje)
        {
            decimal descuento = 0m;

            // Si hay descuento en monto, usarlo directamente
            if (descuentoMonto.HasValue && descuentoMonto.Value > 0)
            {
                descuento = descuentoMonto.Value;
            }
            // Si hay descuento en porcentaje, calcularlo
            else if (descuentoPorcentaje.HasValue && descuentoPorcentaje.Value > 0)
            {
                // Validar que el porcentaje no exceda el máximo permitido
                if (descuentoPorcentaje.Value > ConstantesFacturacion.DESCUENTO_MAXIMO_PORCENTAJE)
                {
                    throw new ArgumentException(
                        $"El descuento no puede ser mayor al {ConstantesFacturacion.DESCUENTO_MAXIMO_PORCENTAJE}%");
                }

                descuento = Math.Round(subtotal * (descuentoPorcentaje.Value / 100m), 2);
            }

            // Validar que el descuento no sea mayor al subtotal
            if (descuento > subtotal)
            {
                throw new ArgumentException("El descuento no puede ser mayor al subtotal");
            }

            return descuento;
        }

        /// <summary>
        /// Capitaliza la primera letra de una palabra
        /// </summary>
        private string CapitalizarPalabra(string palabra)
        {
            if (string.IsNullOrEmpty(palabra))
                return palabra;

            return char.ToUpper(palabra[0]) + palabra.Substring(1).ToLower();
        }

        /// <summary>
        /// Valida que un servicio esté listo para ser facturado
        /// Implementa RN-018: No se puede facturar sin docs completos
        /// </summary>
        public bool ValidarServicioParaFacturar(ServicioFunerario servicio, bool todosDocumentosCompletos)
        {
            if (servicio == null)
                return false;

            // El servicio debe tener estado válido
            if (servicio.Estado != "EnProceso" && servicio.Estado != "Completado")
                return false;

            // Todos los documentos obligatorios deben estar completos (RN-018)
            if (!todosDocumentosCompletos)
                return false;

            return true;
        }

        /// <summary>
        /// Calcula el total rápido sin detalles (para estimaciones)
        /// </summary>
        public decimal CalcularTotalRapido(string paquete, bool ceremoniaReligiosa,
            bool gestionDocumental, decimal descuento = 0)
        {
            decimal subtotal = ObtenerPrecioPaquete(paquete);

            if (ceremoniaReligiosa)
                subtotal += ConstantesFacturacion.PRECIO_CEREMONIA_RELIGIOSA;

            if (gestionDocumental)
                subtotal += ConstantesFacturacion.PRECIO_GESTION_DOCUMENTAL;

            subtotal -= descuento;

            decimal itbms = Math.Round(subtotal * ConstantesFacturacion.PORCENTAJE_ITBMS, 2);

            return subtotal + itbms;
        }
    }
}
