using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Funeraria.Dominio.Entidades;
using Funeraria.Dominio.Interfaces;
using Funeraria.Infraestructura.Persistencia;

namespace Funeraria.Infraestructura.Repositorios
{
    /// <summary>
    /// Implementación del repositorio de facturación
    /// Maneja el acceso a datos para facturas y pagos (CU-019 a CU-022)
    /// </summary>
    public class FacturacionRepository : IFacturacionRepository
    {
        private readonly FunerariaDbContext _context;

        public FacturacionRepository(FunerariaDbContext context)
        {
            _context = context;
        }

        // CU-019: Generar factura
        public async Task<int> GenerarFacturaAsync(Factura factura)
        {
            try
            {
                // Generar número de factura único (RN-016)
                factura.NumeroFactura = await GenerarNumeroFacturaAsync();
                factura.FechaEmision = DateTime.Now;
                factura.MontoPagado = 0;
                factura.SaldoPendiente = factura.Total;
                factura.Pagada = false;

                _context.Facturas.Add(factura);
                await _context.SaveChangesAsync();

                return factura.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar la factura: {ex.Message}", ex);
            }
        }

        public async Task<Factura> ObtenerPorIdAsync(int id)
        {
            try
            {
                return await _context.Facturas
                    .FirstOrDefaultAsync(f => f.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la factura: {ex.Message}", ex);
            }
        }

        public async Task<Factura> ObtenerPorServicioAsync(int servicioId)
        {
            try
            {
                return await _context.Facturas
                    .FirstOrDefaultAsync(f => f.ServicioId == servicioId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener factura del servicio: {ex.Message}", ex);
            }
        }

        // CU-020: Consultar facturas
        public async Task<List<Factura>> ConsultarFacturasAsync(bool? pagadas = null,
            DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            try
            {
                var query = _context.Facturas.AsQueryable();

                // Filtro por estado de pago
                if (pagadas.HasValue)
                {
                    query = query.Where(f => f.Pagada == pagadas.Value);
                }

                // Filtro por fecha desde
                if (fechaDesde.HasValue)
                {
                    query = query.Where(f => f.FechaEmision >= fechaDesde.Value);
                }

                // Filtro por fecha hasta
                if (fechaHasta.HasValue)
                {
                    query = query.Where(f => f.FechaEmision <= fechaHasta.Value);
                }

                return await query
                    .OrderByDescending(f => f.FechaEmision)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al consultar facturas: {ex.Message}", ex);
            }
        }

        // CU-021: Registrar pago
        public async Task RegistrarPagoAsync(Pago pago)
        {
            try
            {
                pago.FechaPago = DateTime.Now;

                _context.Pagos.Add(pago);
                await _context.SaveChangesAsync();

                // Actualizar estado de la factura
                var factura = await ObtenerPorIdAsync(pago.FacturaId);

                if (factura != null)
                {
                    factura.MontoPagado += pago.Monto;
                    factura.SaldoPendiente = factura.Total - factura.MontoPagado;
                    factura.Pagada = factura.SaldoPendiente <= 0;

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar el pago: {ex.Message}", ex);
            }
        }

        public async Task<List<Pago>> ObtenerPagosPorFacturaAsync(int facturaId)
        {
            try
            {
                return await _context.Pagos
                    .Where(p => p.FacturaId == facturaId)
                    .OrderByDescending(p => p.FechaPago)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener pagos: {ex.Message}", ex);
            }
        }

        public async Task ActualizarEstadoPagoAsync(int facturaId, decimal montoPagado,
            decimal saldoPendiente, bool pagada)
        {
            try
            {
                var factura = await ObtenerPorIdAsync(facturaId);

                if (factura == null)
                {
                    throw new Exception($"No se encontró la factura con ID {facturaId}");
                }

                factura.MontoPagado = montoPagado;
                factura.SaldoPendiente = saldoPendiente;
                factura.Pagada = pagada;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar estado de pago: {ex.Message}", ex);
            }
        }

        // Generar número de factura único (RN-016)
        public async Task<string> GenerarNumeroFacturaAsync()
        {
            var año = DateTime.Now.Year;
            var mes = DateTime.Now.Month;

            var ultimaFactura = await _context.Facturas
                .Where(f => f.NumeroFactura.StartsWith($"FAC-{año}{mes:D2}"))
                .OrderByDescending(f => f.NumeroFactura)
                .FirstOrDefaultAsync();

            int numeroConsecutivo = 1;

            if (ultimaFactura != null)
            {
                // Extraer el número (FAC-202511-001 -> 001)
                var partes = ultimaFactura.NumeroFactura.Split('-');
                if (partes.Length == 3 && int.TryParse(partes[2], out int numero))
                {
                    numeroConsecutivo = numero + 1;
                }
            }

            return $"FAC-{año}{mes:D2}-{numeroConsecutivo:D3}";
        }

        // Para dashboard: obtener facturación del mes
        public async Task<decimal> ObtenerFacturacionMesAsync()
        {
            try
            {
                var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var finMes = inicioMes.AddMonths(1).AddDays(-1);

                return await _context.Facturas
                    .Where(f => f.FechaEmision >= inicioMes && f.FechaEmision <= finMes)
                    .SumAsync(f => f.Total);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener facturación del mes: {ex.Message}", ex);
            }
        }
    }
}

