using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Facturacion
{
    /// <summary>
    /// Constantes para los cálculos de facturación
    /// Implementa las reglas de negocio RN-003 a RN-007 y RN-017
    /// </summary>
    public static class ConstantesFacturacion
    {
        // RN-003: Precio del paquete Básico
        public const decimal PRECIO_PAQUETE_BASICO = 1200m;

        // RN-004: Precio del paquete Premium
        public const decimal PRECIO_PAQUETE_PREMIUM = 2500m;

        // RN-005: Precio del paquete Deluxe
        public const decimal PRECIO_PAQUETE_DELUXE = 4000m;

        // RN-006: Costo adicional de ceremonia religiosa
        public const decimal PRECIO_CEREMONIA_RELIGIOSA = 300m;

        // Costo adicional de gestión documental completa
        public const decimal PRECIO_GESTION_DOCUMENTAL = 300m;

        // RN-007 y RN-017: Porcentaje de ITBMS (7%)
        public const decimal PORCENTAJE_ITBMS = 0.07m;

        // Descuento máximo permitido (en porcentaje)
        public const decimal DESCUENTO_MAXIMO_PORCENTAJE = 20m;
    }
}
