using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funeraria.Aplicacion.DTOs.Inventario;

namespace Funeraria.Aplicacion.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de inventario
    /// Implementa los casos de uso CU-014 a CU-018
    /// </summary>
    public interface IInventarioService
    {
        // CU-014: Consultar inventario
        Task<List<ArticuloInventarioDto>> ConsultarInventarioAsync(string categoria = null,
            string estadoStock = null, string buscar = null);
        Task<ArticuloInventarioDto> CrearArticuloAsync(CrearArticuloRequest request);


        // CU-015: Registrar entrada de inventario
        Task RegistrarEntradaAsync(MovimientoInventarioRequest request);

        // CU-016: Registrar salida de inventario
        Task RegistrarSalidaAsync(MovimientoInventarioRequest request);

        // CU-017: Generar alerta de stock bajo
        Task<List<ArticuloInventarioDto>> GenerarAlertasStockBajoAsync();

        // CU-018: Actualizar artículo de inventario
        Task ActualizarArticuloAsync(int id, ArticuloInventarioDto articulo);
    }
}
