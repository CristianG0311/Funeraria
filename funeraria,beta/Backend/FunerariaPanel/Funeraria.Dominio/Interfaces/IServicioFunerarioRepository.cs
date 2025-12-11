using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funeraria.Dominio.Entidades;

namespace Funeraria.Dominio.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de servicios funerarios
    /// Define las operaciones de acceso a datos para servicios
    /// </summary>
    public interface IServicioFunerarioRepository
    {
        // Registrar un nuevo servicio (CU-005)
        Task<int> RegistrarAsync(ServicioFunerario servicio);

        // Obtener servicio por ID (CU-006)
        Task<ServicioFunerario> ObtenerPorIdAsync(int id);

        // Obtener servicio por código de expediente
        Task<ServicioFunerario> ObtenerPorExpedienteAsync(string codigoExpediente);

        // Listar servicios con filtros (CU-006, CU-009)
        Task<List<ServicioFunerario>> ListarAsync(string buscar = null, string estado = null,
            DateTime? fechaDesde = null, DateTime? fechaHasta = null, string paquete = null);

        // Actualizar estado del servicio (CU-007)
        Task ActualizarEstadoAsync(int id, string nuevoEstado, string observaciones);

        // Actualizar servicio completo
        Task ActualizarAsync(ServicioFunerario servicio);

        // Cancelar servicio (CU-008)
        Task CancelarAsync(int id);

        // Obtener servicios activos para dashboard
        Task<int> ContarServiciosActivosAsync();

        // Obtener servicios recientes
        Task<List<ServicioFunerario>> ObtenerRecientesAsync(int cantidad);
    }
}

