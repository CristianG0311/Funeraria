using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Funeraria.Dominio.Entidades;

namespace Funeraria.Dominio.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de inventario
    /// </summary>
    public interface IInventarioRepository
    {
        // Consultar inventario (CU-014)
        Task<List<ArticuloInventario>> ConsultarInventarioAsync(string categoria = null,
            string estadoStock = null, string buscar = null);

        // Obtener artículo por ID
        Task<ArticuloInventario> ObtenerPorIdAsync(int id);

        // Registrar entrada de inventario (CU-015)
        Task RegistrarEntradaAsync(int articuloId, int cantidad, string motivo, string observaciones);

        // Registrar salida de inventario (CU-016)
        Task RegistrarSalidaAsync(int articuloId, int cantidad, string motivo, string observaciones);

        // Actualizar artículo (CU-018)
        Task ActualizarArticuloAsync(ArticuloInventario articulo);

        // Consultar inventario crítico
        Task<List<ArticuloInventario>> ConsultarInventarioCriticoAsync();

        // Contar artículos críticos para dashboard
        Task<int> ContarArticulosCriticosAsync();

        // Generar alerta de stock bajo (CU-017)
        Task<List<ArticuloInventario>> GenerarAlertasStockBajoAsync();
    }
}
