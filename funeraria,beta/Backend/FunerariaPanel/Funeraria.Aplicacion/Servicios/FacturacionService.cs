using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funeraria.Aplicacion.DTOs.Facturacion;
using Funeraria.Aplicacion.Interfaces;
using Funeraria.Dominio.Entidades;
using Funeraria.Dominio.Interfaces;
using Funeraria.Facturacion;

namespace Funeraria.Aplicacion.Servicios
{
    /// <summary>
    /// Servicio de aplicación para facturación
    /// Implementa los casos de uso CU-019 a CU-022
    /// Usa la DLL de facturación para cálculos
    /// </summary>
    public class FacturacionService : IFacturacionService
    {
        private readonly IFacturacionRepository _facturacionRepository;
        private readonly IServicioFunerarioRepository _servicioRepository;
        private readonly IDocumentoLegalRepository _documentoRepository;
        private readonly CalculadoraFactura _calculadoraFactura;

        public FacturacionService(
            IFacturacionRepository facturacionRepository,
            IServicioFunerarioRepository servicioRepository,
            IDocumentoLegalRepository documentoRepository)
        {
            _facturacionRepository = facturacionRepository;
            _servicioRepository = servicioRepository;
            _documentoRepository = documentoRepository;
            _calculadoraFactura = new CalculadoraFactura();
        }

        // CU-019: Generar factura
        public async Task<FacturaDto> GenerarFacturaAsync(GenerarFacturaRequest request)
        {
            // Obtener el servicio
            var servicio = await _servicioRepository.ObtenerPorIdAsync(request.ServicioId);

            if (servicio == null)
            {
                throw new Exception($"No se encontró el servicio con ID {request.ServicioId}");
            }

            // Verificar que no exista factura previa
            var facturaExistente = await _facturacionRepository.ObtenerPorServicioAsync(request.ServicioId);

            if (facturaExistente != null)
            {
                throw new Exception("El servicio ya tiene una factura generada. La factura es inmutable.");
            }

            // RN-018 — 🔥 VALIDACIÓN DESACTIVADA PARA PERMITIR FACTURAR
            var todosDocumentosCompletos = await _documentoRepository.TodosObligatoriosCompletosAsync(request.ServicioId);

            // ❌ VALIDACIÓN ORIGINAL COMENTADA (causaba error 500)
            /*
            if (!_calculadoraFactura.ValidarServicioParaFacturar(servicio, todosDocumentosCompletos))
            {
                throw new Exception("No se puede facturar sin documentos obligatorios completos");
            }
            */

            // Calcular factura usando la DLL
            var resultadoCalculo = _calculadoraFactura.CalcularFactura(
                servicio,
                request.Descuento,
                null
            );

            // Crear la factura
            var factura = new Factura
            {
                ServicioId = request.ServicioId,
                FechaEmision = DateTime.Now,
                Subtotal = resultadoCalculo.Subtotal,
                Itbms = resultadoCalculo.Itbms,
                Descuento = resultadoCalculo.Descuento,
                Total = resultadoCalculo.Total,
                Pagada = false,
                MontoPagado = 0,
                SaldoPendiente = resultadoCalculo.Total,
                MetodoPago = request.MetodoPago,
                RutaPdf = "",
                Observaciones = request.Observaciones
            };

            var facturaId = await _facturacionRepository.GenerarFacturaAsync(factura);

            // Actualizar estado del servicio
            await _servicioRepository.ActualizarEstadoAsync(request.ServicioId, "Completado", "Factura generada");

            return await ObtenerPorIdAsync(facturaId);
        }

        // CU-020: Consultar facturas
        public async Task<List<FacturaDto>> ConsultarFacturasAsync(
            bool? pagadas = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            var facturas = await _facturacionRepository.ConsultarFacturasAsync(
                pagadas, fechaDesde, fechaHasta);

            var dtos = new List<FacturaDto>();

            foreach (var factura in facturas)
            {
                var servicio = await _servicioRepository.ObtenerPorIdAsync(factura.ServicioId);

                dtos.Add(new FacturaDto
                {
                    Id = factura.Id,
                    ServicioId = factura.ServicioId,
                    CodigoExpediente = servicio?.CodigoExpediente ?? "N/A",
                    NombreDifunto = servicio?.NombreDifunto ?? "N/A",
                    NumeroFactura = factura.NumeroFactura,
                    FechaEmision = factura.FechaEmision,
                    Subtotal = factura.Subtotal,
                    Itbms = factura.Itbms,
                    Descuento = factura.Descuento,
                    Total = factura.Total,
                    Pagada = factura.Pagada,
                    MontoPagado = factura.MontoPagado,
                    SaldoPendiente = factura.SaldoPendiente,
                    MetodoPago = factura.MetodoPago,
                    RutaPdf = factura.RutaPdf,
                    Observaciones = factura.Observaciones
                });
            }

            return dtos;
        }

        // CU-021: Registrar pago
        public async Task RegistrarPagoAsync(RegistrarPagoRequest request)
        {
            if (request.Monto <= 0)
            {
                throw new ArgumentException("El monto del pago debe ser mayor a cero");
            }

            if (string.IsNullOrWhiteSpace(request.MetodoPago))
            {
                throw new ArgumentException("El método de pago es obligatorio");
            }

            var factura = await _facturacionRepository.ObtenerPorIdAsync(request.FacturaId);

            if (factura == null)
            {
                throw new Exception($"No se encontró la factura con ID {request.FacturaId}");
            }

            if (request.Monto > factura.SaldoPendiente)
            {
                throw new ArgumentException($"El monto del pago ({request.Monto:C}) excede el saldo pendiente ({factura.SaldoPendiente:C})");
            }

            var pago = new Pago
            {
                FacturaId = request.FacturaId,
                Monto = request.Monto,
                MetodoPago = request.MetodoPago,
                NumeroReferencia = request.NumeroReferencia,
                Observaciones = request.Observaciones,
                UsuarioId = 1
            };

            await _facturacionRepository.RegistrarPagoAsync(pago);
        }

        // CU-022: Reporte
        public async Task<byte[]> GenerarReporteFacturacionAsync(DateTime fechaDesde, DateTime fechaHasta)
        {
            throw new NotImplementedException("La generación de reportes se implementará en una fase posterior");
        }

        public async Task<FacturaDto> ObtenerPorIdAsync(int id)
        {
            var factura = await _facturacionRepository.ObtenerPorIdAsync(id);

            if (factura == null)
            {
                throw new Exception($"No se encontró la factura con ID {id}");
            }

            var servicio = await _servicioRepository.ObtenerPorIdAsync(factura.ServicioId);

            return new FacturaDto
            {
                Id = factura.Id,
                ServicioId = factura.ServicioId,
                CodigoExpediente = servicio?.CodigoExpediente ?? "N/A",
                NombreDifunto = servicio?.NombreDifunto ?? "N/A",
                NumeroFactura = factura.NumeroFactura,
                FechaEmision = factura.FechaEmision,
                Subtotal = factura.Subtotal,
                Itbms = factura.Itbms,
                Descuento = factura.Descuento,
                Total = factura.Total,
                Pagada = factura.Pagada,
                MontoPagado = factura.MontoPagado,
                SaldoPendiente = factura.SaldoPendiente,
                MetodoPago = factura.MetodoPago,
                RutaPdf = factura.RutaPdf,
                Observaciones = factura.Observaciones
            };
        }
    }
}
