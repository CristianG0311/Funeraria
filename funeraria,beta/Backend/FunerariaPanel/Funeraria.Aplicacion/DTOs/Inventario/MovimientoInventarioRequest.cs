using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Aplicacion.DTOs.Inventario
{
    /// <summary>
    /// DTO para registrar entradas y salidas de inventario (CU-015, CU-016)
    /// </summary>
    public class MovimientoInventarioRequest
    {
        public int ArticuloId { get; set; }
        public int Cantidad { get; set; }
        public string TipoMovimiento { get; set; } // "Entrada" o "Salida"
        public string Motivo { get; set; }
        public string Observaciones { get; set; }
    }
}
