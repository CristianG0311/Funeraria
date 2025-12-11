using Microsoft.AspNetCore.Mvc;
using Funeraria.Aplicacion.Interfaces;
using Funeraria.Aplicacion.DTOs.Documentos;

namespace Funeraria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentosController : ControllerBase
    {
        private readonly IDocumentoService _documentoService;

        public DocumentosController(IDocumentoService documentoService)
        {
            _documentoService = documentoService;
        }

        [HttpGet]
        public async Task<ActionResult> ListarDocumentos(
            [FromQuery] int? servicioId,
            [FromQuery] string? estado,
            [FromQuery] string? tipoDocumento)
        {
            List<DocumentoLegalDto> documentos;

            if (servicioId.HasValue)
                documentos = await _documentoService.ListarPorServicioAsync(servicioId.Value);
            else
                documentos = await _documentoService.ListarTodosAsync(); // 🔥 USAR EL NUEVO MÉTODO

            if (!string.IsNullOrWhiteSpace(estado))
                documentos = documentos.Where(d => d.Estado == estado).ToList();

            if (!string.IsNullOrWhiteSpace(tipoDocumento))
                documentos = documentos.Where(d => d.TipoDocumento == tipoDocumento).ToList();

            return Ok(documentos);
        }

        [HttpPost]
        public async Task<ActionResult> CrearDocumento([FromBody] CrearDocumentoRequest request)
        {
            await _documentoService.CrearDocumentoAsync(request);
            return Ok(new { mensaje = "Documento creado correctamente" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarEstado(int id, [FromBody] ActualizarDocumentoRequest request)
        {
            await _documentoService.ActualizarEstadoAsync(id, request);
            return Ok(new { mensaje = "Documento actualizado correctamente" });
        }

        [HttpGet("pendientes")]
        public async Task<ActionResult> ListarPendientes()
        {
            return Ok(await _documentoService.ListarPendientesAsync());
        }
    }
}
