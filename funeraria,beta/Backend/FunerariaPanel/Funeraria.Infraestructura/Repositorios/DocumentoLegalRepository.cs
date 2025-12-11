using Microsoft.EntityFrameworkCore;
using Funeraria.Dominio.Entidades;
using Funeraria.Dominio.Interfaces;
using Funeraria.Infraestructura.Persistencia;

namespace Funeraria.Infraestructura.Repositorios
{
    public class DocumentoLegalRepository : IDocumentoLegalRepository
    {
        private readonly FunerariaDbContext _context;

        public DocumentoLegalRepository(FunerariaDbContext context)
        {
            _context = context;
        }

        public async Task<int> RegistrarAsync(DocumentoLegal documento)
        {
            documento.FechaActualizacion = DateTime.Now;

            _context.DocumentosLegales.Add(documento);
            await _context.SaveChangesAsync();

            return documento.Id;
        }

        public async Task<DocumentoLegal> ObtenerPorIdAsync(int id)
        {
            return await _context.DocumentosLegales.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<DocumentoLegal>> ListarPorServicioAsync(int servicioId)
        {
            return await _context.DocumentosLegales
                .Where(d => d.ServicioId == servicioId)
                .OrderBy(d => d.TipoDocumento)
                .ToListAsync();
        }

        public async Task<List<DocumentoLegal>> ListarPendientesAsync()
        {
            return await _context.DocumentosLegales
                .Where(d => d.Estado == "Pendiente" || d.Estado == "EnTramite")
                .OrderBy(d => d.FechaActualizacion)
                .ToListAsync();
        }

        // 🔥 NUEVO - Listar todos los documentos
        public async Task<List<DocumentoLegal>> ListarTodosAsync()
        {
            return await _context.DocumentosLegales
                .OrderByDescending(d => d.FechaActualizacion)
                .ToListAsync();
        }

        public async Task ActualizarEstadoAsync(int id, string numeroDocumento,
            DateTime? fechaEmision, string entidadEmisora, string estado, string observaciones)
        {
            var documento = await ObtenerPorIdAsync(id);
            
            if (documento == null)
                throw new ArgumentException($"No se encontró el documento con ID: {id}");

            // Actualizar todos los campos
            documento.NumeroDocumento = numeroDocumento ?? documento.NumeroDocumento;
            documento.FechaEmision = fechaEmision ?? documento.FechaEmision;
            documento.EntidadEmisora = entidadEmisora ?? documento.EntidadEmisora;
            documento.Estado = estado; // Siempre actualizar el estado
            documento.Observaciones = observaciones ?? documento.Observaciones;
            documento.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> TodosObligatoriosCompletosAsync(int servicioId)
        {
            var docs = await _context.DocumentosLegales
                .Where(d => d.ServicioId == servicioId && d.EsObligatorio)
                .ToListAsync();

            return docs.Any() && docs.All(d => d.Estado == "Completo");
        }

        public async Task<int> ContarPendientesAsync()
        {
            return await _context.DocumentosLegales
                .Where(d => d.Estado == "Pendiente" || d.Estado == "EnTramite")
                .CountAsync();
        }
    }
}
