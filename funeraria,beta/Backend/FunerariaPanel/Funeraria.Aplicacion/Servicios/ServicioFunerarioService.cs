using Funeraria.Aplicacion.DTOs.Servicios;
using Funeraria.Aplicacion.Interfaces;
using Funeraria.Dominio.Entidades;
using Funeraria.Dominio.Interfaces;
using Funeraria.Facturacion;

namespace Funeraria.Aplicacion.Servicios
{
    /// <summary>
    /// Servicio de aplicación para servicios funerarios
    /// Implementa los casos de uso CU-005 a CU-009
    /// Aplica validaciones y reglas de negocio
    /// </summary>
    public class ServicioFunerarioService : IServicioFunerarioService
    {
        private readonly IServicioFunerarioRepository _servicioRepository;
        private readonly IDocumentoLegalRepository _documentoRepository;
        private readonly CalculadoraFactura _calculadoraFactura;

        public ServicioFunerarioService(
            IServicioFunerarioRepository servicioRepository,
            IDocumentoLegalRepository documentoRepository)
        {
            _servicioRepository = servicioRepository;
            _documentoRepository = documentoRepository;
            _calculadoraFactura = new CalculadoraFactura();
        }

        // CU-005: Registrar servicio funerario
        public async Task<ServicioFunerarioDto> RegistrarServicioAsync(RegistrarServicioRequest request)
        {
            // Validaciones
            ValidarDatosServicio(request);

            // Crear entidad
            var servicio = new ServicioFunerario
            {
                NombreDifunto = request.NombreDifunto,
                CedulaDifunto = request.CedulaDifunto,
                FechaFallecimiento = request.FechaFallecimiento,
                Edad = request.Edad,
                LugarFallecimiento = request.LugarFallecimiento,
                NombreFamiliar = request.NombreFamiliar,
                Parentesco = request.Parentesco,
                TelefonoFamiliar = request.TelefonoFamiliar,
                EmailFamiliar = request.EmailFamiliar,
                Paquete = request.Paquete.ToLower(),
                TipoServicio = request.TipoServicio?.ToLower(),
                SalaVelacion = request.SalaVelacion,
                CeremoniaReligiosa = request.CeremoniaReligiosa,
                GestionDocumentalCompleta = request.GestionDocumentalCompleta,
                Observaciones = request.Observaciones
            };

            // Calcular el total usando la DLL de facturación
            servicio.Total = _calculadoraFactura.CalcularTotalRapido(
                servicio.Paquete,
                servicio.CeremoniaReligiosa,
                servicio.GestionDocumentalCompleta
            );

            // Guardar en la base de datos
            var servicioId = await _servicioRepository.RegistrarAsync(servicio);

            // Crear documentos requeridos automáticamente (RN-009, RN-010, RN-011)
            // TEMPORAL: Desactivado para evitar error, se puede activar después
            // await CrearDocumentosRequeridosAsync(servicioId);

            // Obtener el servicio completo
            var servicioCreado = await _servicioRepository.ObtenerPorIdAsync(servicioId);

            return await MapearADto(servicioCreado);
        }

        // CU-006: Listar servicios con filtros
        public async Task<List<ServicioFunerarioDto>> ListarServiciosAsync(FiltroServiciosRequest filtro)
        {
            var servicios = await _servicioRepository.ListarAsync(
                filtro.Buscar,
                filtro.Estado,
                filtro.FechaDesde,
                filtro.FechaHasta,
                filtro.Paquete
            );

            var dtos = new List<ServicioFunerarioDto>();

            foreach (var servicio in servicios)
            {
                dtos.Add(await MapearADto(servicio));
            }

            return dtos;
        }

        // Obtener servicio por ID
        public async Task<ServicioFunerarioDto> ObtenerPorIdAsync(int id)
        {
            var servicio = await _servicioRepository.ObtenerPorIdAsync(id);

            if (servicio == null)
            {
                throw new Exception($"No se encontró el servicio con ID {id}");
            }

            return await MapearADto(servicio);
        }

        // CU-007: Cambiar estado del servicio
        public async Task CambiarEstadoAsync(int id, CambiarEstadoServicioRequest request)
        {
            // Validar que el estado sea válido
            var estadosValidos = new[] { "Registrado", "EnProceso", "Completado", "Cancelado" };

            if (!estadosValidos.Contains(request.NuevoEstado))
            {
                throw new Exception($"Estado inválido: {request.NuevoEstado}");
            }

            await _servicioRepository.ActualizarEstadoAsync(id, request.NuevoEstado, request.Observaciones);
        }

        // CU-008: Cancelar servicio
        public async Task CancelarServicioAsync(int id)
        {
            await _servicioRepository.CancelarAsync(id);
        }

        // CU-009: Buscar por expediente
        public async Task<ServicioFunerarioDto> BuscarPorExpedienteAsync(string codigoExpediente)
        {
            var servicio = await _servicioRepository.ObtenerPorExpedienteAsync(codigoExpediente);

            if (servicio == null)
            {
                throw new Exception($"No se encontró el expediente {codigoExpediente}");
            }

            return await MapearADto(servicio);
        }

        // Métodos privados de validación y mapeo

        private void ValidarDatosServicio(RegistrarServicioRequest request)
        {
            // Validar campos requeridos
            if (string.IsNullOrWhiteSpace(request.NombreDifunto))
            {
                throw new ArgumentException("El nombre del difunto es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(request.NombreFamiliar))
            {
                throw new ArgumentException("El nombre del familiar es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(request.TelefonoFamiliar))
            {
                throw new ArgumentException("El teléfono del familiar es obligatorio");
            }

            // RN-001: Fecha de fallecimiento no puede ser futura
            if (request.FechaFallecimiento > DateTime.Now.Date)
            {
                throw new ArgumentException("La fecha de fallecimiento no puede ser futura");
            }

            // RN-002: Edad debe estar entre 0 y 150 años
            if (request.Edad < 0 || request.Edad > 150)
            {
                throw new ArgumentException("La edad debe estar entre 0 y 150 años");
            }

            // Validar paquete
            ValidadorFacturacion.ValidarPaquete(request.Paquete);
        }

        private async Task CrearDocumentosRequeridosAsync(int servicioId)
        {
            // RN-009: Certificado médico es obligatorio
            await _documentoRepository.RegistrarAsync(new DocumentoLegal
            {
                ServicioId = servicioId,
                TipoDocumento = "CertificadoMedico",
                Estado = "Pendiente",
                EsObligatorio = true
            });

            // RN-010: Acta de defunción es obligatoria
            await _documentoRepository.RegistrarAsync(new DocumentoLegal
            {
                ServicioId = servicioId,
                TipoDocumento = "ActaDefuncion",
                Estado = "Pendiente",
                EsObligatorio = true
            });

            // RN-011: Permiso de inhumación/cremación es obligatorio
            await _documentoRepository.RegistrarAsync(new DocumentoLegal
            {
                ServicioId = servicioId,
                TipoDocumento = "PermisoInhumacion",
                Estado = "Pendiente",
                EsObligatorio = true
            });

            // Identificación del familiar (obligatorio)
            await _documentoRepository.RegistrarAsync(new DocumentoLegal
            {
                ServicioId = servicioId,
                TipoDocumento = "IdentificacionFamiliar",
                Estado = "Pendiente",
                EsObligatorio = true
            });
        }

        private async Task<ServicioFunerarioDto> MapearADto(ServicioFunerario servicio)
        {
            // Obtener información de documentos (con manejo de error)
            int documentosCompletos = 0;
            int documentosPendientes = 0;
            int totalDocumentos = 0;

            try
            {
                var documentos = await _documentoRepository.ListarPorServicioAsync(servicio.Id);
                documentosCompletos = documentos.Count(d => d.Estado == "Completo");
                documentosPendientes = documentos.Count(d => d.Estado == "Pendiente" || d.Estado == "EnTramite");
                totalDocumentos = documentos.Count;
            }
            catch
            {
                // Si hay error al obtener documentos, simplemente dejar en 0
            }

            return new ServicioFunerarioDto
            {
                Id = servicio.Id,
                CodigoExpediente = servicio.CodigoExpediente,
                NombreDifunto = servicio.NombreDifunto,
                CedulaDifunto = servicio.CedulaDifunto,
                FechaFallecimiento = servicio.FechaFallecimiento,
                Edad = servicio.Edad,
                LugarFallecimiento = servicio.LugarFallecimiento,
                NombreFamiliar = servicio.NombreFamiliar,
                Parentesco = servicio.Parentesco,
                TelefonoFamiliar = servicio.TelefonoFamiliar,
                EmailFamiliar = servicio.EmailFamiliar,
                Paquete = servicio.Paquete,
                TipoServicio = servicio.TipoServicio,
                SalaVelacion = servicio.SalaVelacion,
                CeremoniaReligiosa = servicio.CeremoniaReligiosa,
                GestionDocumentalCompleta = servicio.GestionDocumentalCompleta,
                Estado = servicio.Estado,
                Total = servicio.Total,
                FechaRegistro = servicio.FechaRegistro,
                Observaciones = servicio.Observaciones,
                DocumentosPendientes = documentosPendientes,
                DocumentosCompletos = documentosCompletos,
                TotalDocumentos = totalDocumentos
            };
        }
    }
}
