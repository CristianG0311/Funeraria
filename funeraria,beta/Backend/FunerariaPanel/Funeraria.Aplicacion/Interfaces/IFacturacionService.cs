using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funeraria.Aplicacion.DTOs.Facturacion;

namespace Funeraria.Aplicacion.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de facturación
    /// Implementa los casos de uso CU-019 a CU-022
    /// </summary>
    public interface IFacturacionService
    {
        // CU-019: Generar factura
        Task<FacturaDto> GenerarFacturaAsync(GenerarFacturaRequest request);

        // CU-020: Consultar facturas
        Task<List<FacturaDto>> ConsultarFacturasAsync(bool? pagadas = null,
            DateTime? fechaDesde = null, DateTime? fechaHasta = null);

        // CU-021: Registrar pago
        Task RegistrarPagoAsync(RegistrarPagoRequest request);

        // CU-022: Generar reporte de facturación
        Task<byte[]> GenerarReporteFacturacionAsync(DateTime fechaDesde, DateTime fechaHasta);

        // Obtener factura por ID
        Task<FacturaDto> ObtenerPorIdAsync(int id);
    }
}
