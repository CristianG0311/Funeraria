using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funeraria.Aplicacion.DTOs.Servicios;

namespace Funeraria.Aplicacion.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de aplicación de servicios funerarios
    /// Implementa los casos de uso CU-005 a CU-009
    /// </summary>
    public interface IServicioFunerarioService
    {
        // CU-005: Registrar servicio funerario
        Task<ServicioFunerarioDto> RegistrarServicioAsync(RegistrarServicioRequest request);

        // CU-006: Consultar servicios
        Task<List<ServicioFunerarioDto>> ListarServiciosAsync(FiltroServiciosRequest filtro);

        // Obtener servicio por ID
        Task<ServicioFunerarioDto> ObtenerPorIdAsync(int id);

        // CU-007: Actualizar estado de servicio
        Task CambiarEstadoAsync(int id, CambiarEstadoServicioRequest request);

        // CU-008: Cancelar servicio
        Task CancelarServicioAsync(int id);

        // CU-009: Buscar servicio por expediente
        Task<ServicioFunerarioDto> BuscarPorExpedienteAsync(string codigoExpediente);
    }
}
