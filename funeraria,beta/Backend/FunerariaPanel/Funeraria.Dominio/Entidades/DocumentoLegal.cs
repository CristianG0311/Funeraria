namespace Funeraria.Dominio.Entidades
{
    public class DocumentoLegal
    {
        public int Id { get; set; }

        public int ServicioId { get; set; }
        public ServicioFunerario Servicio { get; set; }

        public string TipoDocumento { get; set; }

        public string NumeroDocumento { get; set; }

        public DateTime? FechaEmision { get; set; }

        public string EntidadEmisora { get; set; }

        public string Estado { get; set; }

        public string Observaciones { get; set; }

        public string? RutaArchivo { get; set; }

        public bool EsObligatorio { get; set; }

        public DateTime FechaActualizacion { get; set; }
    }
}
