using Funeraria.Aplicacion.DTOs.Documentos;
using Funeraria.Aplicacion.Interfaces;
using Funeraria.Dominio.Entidades;
using Funeraria.Dominio.Interfaces;

namespace Funeraria.Aplicacion.Servicios
{
    public class DocumentoService : IDocumentoService
    {
        private readonly IDocumentoLegalRepository _documentoRepository;
        private readonly IServicioFunerarioRepository _servicioRepository;

        public DocumentoService(
            IDocumentoLegalRepository documentoRepository,
            IServicioFunerarioRepository servicioRepository)
        {
            _documentoRepository = documentoRepository;
            _servicioRepository = servicioRepository;
        }

        public Task CrearDocumentosRequeridosAsync(int servicioId)
        {
            throw new NotImplementedException("Los documentos se crean automáticamente al registrar el servicio");
        }

        public async Task ActualizarEstadoAsync(int documentoId, ActualizarDocumentoRequest request)
        {
            Console.WriteLine($"[DocumentoService] Actualizando documento {documentoId}");
            Console.WriteLine($"[DocumentoService] Estado recibido: '{request.Estado}'");
            
            if (string.IsNullOrWhiteSpace(request.Estado))
                throw new ArgumentException("El estado es obligatorio");

            var estadosValidos = new[] { "Pendiente", "EnTramite", "Completo", "Rechazado" };
            if (!estadosValidos.Contains(request.Estado))
                throw new ArgumentException($"Estado inválido: {request.Estado}");

            if (request.Estado == "Rechazado" && string.IsNullOrWhiteSpace(request.Observaciones))
                throw new ArgumentException("Las observaciones son obligatorias cuando el documento es rechazado");

            Console.WriteLine($"[DocumentoService] Validaciones pasadas. Actualizando a estado: '{request.Estado}'");

            await _documentoRepository.ActualizarEstadoAsync(
                documentoId,
                request.NumeroDocumento,
                request.FechaEmision,
                request.EntidadEmisora,
                request.Estado,
                request.Observaciones
            );
            
            Console.WriteLine($"[DocumentoService] Documento {documentoId} actualizado correctamente");
        }

        public async Task CrearDocumentoAsync(CrearDocumentoRequest request)
        {
            Console.WriteLine($"[DocumentoService] Creando nuevo documento");
            Console.WriteLine($"[DocumentoService] Estado recibido: '{request.Estado}'");
            
            var servicio = await _servicioRepository.ObtenerPorIdAsync(request.ServicioId);
            if (servicio == null)
                throw new ArgumentException("El servicio especificado no existe");

            var estadosValidos = new[] { "Pendiente", "EnTramite", "Completo", "Rechazado" };
            if (!estadosValidos.Contains(request.Estado))
                throw new ArgumentException($"Estado inválido: {request.Estado}");

            var documento = new DocumentoLegal
            {
                ServicioId = request.ServicioId,
                TipoDocumento = request.TipoDocumento,
                NumeroDocumento = request.NumeroDocumento,
                EntidadEmisora = request.EntidadEmisora,
                FechaEmision = request.FechaEmision,
                Estado = request.Estado,
                EsObligatorio = request.EsObligatorio,
                Observaciones = request.Observaciones,
                FechaActualizacion = DateTime.Now
            };

            Console.WriteLine($"[DocumentoService] Documento creado con estado: '{documento.Estado}'");
            
            await _documentoRepository.RegistrarAsync(documento);
            
            Console.WriteLine($"[DocumentoService] Documento guardado en BD");
        }

        public async Task<List<DocumentoLegalDto>> ListarPendientesAsync()
        {
            var documentos = await _documentoRepository.ListarPendientesAsync();
            return await MapearDocumentos(documentos);
        }

        public async Task<List<DocumentoLegalDto>> ListarPorServicioAsync(int servicioId)
        {
            var documentos = await _documentoRepository.ListarPorServicioAsync(servicioId);
            return await MapearDocumentos(documentos);
        }

        // ============================================================
        // NUEVO: LISTAR TODOS LOS DOCUMENTOS (USADO POR EL FRONTEND)
        // ============================================================
        public async Task<List<DocumentoLegalDto>> ListarTodosAsync()
        {
            var documentos = await _documentoRepository.ListarTodosAsync(); // <-- Debe existir en repo
            return await MapearDocumentos(documentos);
        }

        // ============================================================
        // MÉTODO COMPARTIDO PARA MAPEAR ENTIDAD → DTO
        // ============================================================
        private async Task<List<DocumentoLegalDto>> MapearDocumentos(List<DocumentoLegal> documentos)
        {
            var dtos = new List<DocumentoLegalDto>();

            foreach (var documento in documentos)
            {
                var servicio = await _servicioRepository.ObtenerPorIdAsync(documento.ServicioId);

                dtos.Add(new DocumentoLegalDto
                {
                    Id = documento.Id,
                    ServicioId = documento.ServicioId,
                    CodigoExpediente = servicio?.CodigoExpediente ?? "N/A",
                    NombreDifunto = servicio?.NombreDifunto ?? "N/A",
                    TipoDocumento = documento.TipoDocumento,
                    NumeroDocumento = documento.NumeroDocumento,
                    FechaEmision = documento.FechaEmision,
                    EntidadEmisora = documento.EntidadEmisora,
                    Estado = documento.Estado,
                    Observaciones = documento.Observaciones,
                    RutaArchivo = documento.RutaArchivo,
                    EsObligatorio = documento.EsObligatorio,
                    FechaActualizacion = documento.FechaActualizacion
                });
            }

            return dtos;
        }

        public Task<byte[]> GenerarReporteDocumentosAsync(DateTime fechaDesde, DateTime fechaHasta)
        {
            throw new NotImplementedException("La generación de reportes se implementará en una fase posterior");
        }
    }
}
