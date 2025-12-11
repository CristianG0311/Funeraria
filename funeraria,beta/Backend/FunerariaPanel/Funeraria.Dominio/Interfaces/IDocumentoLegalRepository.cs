using Funeraria.Dominio.Entidades;

namespace Funeraria.Dominio.Interfaces
{
    public interface IDocumentoLegalRepository
    {
        Task<int> RegistrarAsync(DocumentoLegal documento);
        Task<DocumentoLegal> ObtenerPorIdAsync(int id);
        Task<List<DocumentoLegal>> ListarPorServicioAsync(int servicioId);
        Task<List<DocumentoLegal>> ListarPendientesAsync();

        // 🔥 MÉTODO NUEVO PARA QUE EL FRONTEND FUNCIONE
        Task<List<DocumentoLegal>> ListarTodosAsync();

        Task ActualizarEstadoAsync(int id, string numeroDocumento, DateTime? fechaEmision,
            string entidadEmisora, string estado, string observaciones);

        Task<bool> TodosObligatoriosCompletosAsync(int servicioId);
        Task<int> ContarPendientesAsync();
    }
}
