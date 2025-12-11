using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Funeraria.Aplicacion.DTOs.Documentos;

namespace Funeraria.Aplicacion.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de documentos
    /// Implementa los casos de uso CU-010 a CU-013
    /// </summary>
    public interface IDocumentoService
    {
        // CU-010: Registrar documentos requeridos (automático al crear servicio)
        Task CrearDocumentosRequeridosAsync(int servicioId);

        // CU-010: Crear documento individual
        Task CrearDocumentoAsync(CrearDocumentoRequest request);

        // CU-011: Actualizar estado de documento
        Task ActualizarEstadoAsync(int documentoId, ActualizarDocumentoRequest request);

        // CU-012: Consultar documentos pendientes
        Task<List<DocumentoLegalDto>> ListarPendientesAsync();

        // Listar documentos por servicio
        Task<List<DocumentoLegalDto>> ListarPorServicioAsync(int servicioId);

        // **NUEVO** → Listar TODOS los documentos (usado por tu frontend)
        Task<List<DocumentoLegalDto>> ListarTodosAsync();

        // CU-013: Generar reporte de documentos
        Task<byte[]> GenerarReporteDocumentosAsync(DateTime fechaDesde, DateTime fechaHasta);
    }
}
