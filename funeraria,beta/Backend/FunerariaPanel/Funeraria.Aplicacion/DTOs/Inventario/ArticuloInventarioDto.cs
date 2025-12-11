using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Aplicacion.DTOs.Inventario
{
    /// <summary>
    /// DTO para artículos de inventario (CU-014 a CU-018)
    /// </summary>
    public class ArticuloInventarioDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public int CantidadDisponible { get; set; }
        public int StockMinimo { get; set; }
        public string EstadoStock { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCosto { get; set; }
        public DateTime FechaActualizacion { get; set; }

     

    }
}
