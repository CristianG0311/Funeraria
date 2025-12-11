using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Funeraria.Dominio.Entidades;

namespace Funeraria.Dominio.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de facturación
    /// </summary>
    public interface IFacturacionRepository
    {
        // Generar factura (CU-019)
        Task<int> GenerarFacturaAsync(Factura factura);

        // Obtener factura por ID
        Task<Factura> ObtenerPorIdAsync(int id);

        // Obtener factura por servicio
        Task<Factura> ObtenerPorServicioAsync(int servicioId);

        // Consultar facturas (CU-020)
        Task<List<Factura>> ConsultarFacturasAsync(bool? pagadas = null,
            DateTime? fechaDesde = null, DateTime? fechaHasta = null);

        // Registrar pago (CU-021)
        Task RegistrarPagoAsync(Pago pago);

        // Obtener pagos de una factura
        Task<List<Pago>> ObtenerPagosPorFacturaAsync(int facturaId);

        // Actualizar estado de pago de factura
        Task ActualizarEstadoPagoAsync(int facturaId, decimal montoPagado, decimal saldoPendiente, bool pagada);

        // Generar número de factura único
        Task<string> GenerarNumeroFacturaAsync();

        // Obtener facturación del mes para dashboard
        Task<decimal> ObtenerFacturacionMesAsync();
    }
}

