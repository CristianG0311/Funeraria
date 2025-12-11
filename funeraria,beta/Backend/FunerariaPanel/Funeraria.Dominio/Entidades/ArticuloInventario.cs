using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Dominio.Entidades
{
    /// <summary>
    /// Representa un artículo del inventario de la funeraria (CU-014 a CU-018)
    /// </summary>
    public class ArticuloInventario
    {
        // Identificador único del artículo
        public int Id { get; set; }

        // Código del artículo (ejemplo: "ATD-PRM-001")
        public string Codigo { get; set; }

        // Nombre del artículo (ejemplo: "Ataúd Premium Caoba")
        public string Nombre { get; set; }

        // Descripción detallada del artículo
        public string Descripcion { get; set; }

        // Categoría: "Ataud", "Urna", "Flores", "Vehiculo", "Accesorios"
        public string Categoria { get; set; }

        // Cantidad disponible en inventario
        public int CantidadDisponible { get; set; }

        // Stock mínimo antes de generar alerta
        public int StockMinimo { get; set; }

        // Estado calculado: "Normal", "Bajo", "Critico", "Agotado"
        public string EstadoStock { get; set; }

        // Precio de venta del artículo
        public decimal PrecioVenta { get; set; }

        // Precio de costo (para cálculos internos)
        public decimal PrecioCosto { get; set; }

        // Fecha de última actualización
        public DateTime FechaActualizacion { get; set; }

    }
}

