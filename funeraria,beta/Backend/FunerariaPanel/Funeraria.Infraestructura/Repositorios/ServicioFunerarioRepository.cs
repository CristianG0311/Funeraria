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
    /// Implementación del repositorio de servicios funerarios
    /// Maneja el acceso a datos para servicios (CU-005 a CU-009)
    /// </summary>
    public class ServicioFunerarioRepository : IServicioFunerarioRepository
    {
        private readonly FunerariaDbContext _context;

        public ServicioFunerarioRepository(FunerariaDbContext context)
        {
            _context = context;
        }

        // CU-005: Registrar servicio
        public async Task<int> RegistrarAsync(ServicioFunerario servicio)
        {
            try
            {
                // Generar código de expediente único
                servicio.CodigoExpediente = await GenerarCodigoExpedienteAsync();
                servicio.FechaRegistro = DateTime.Now;
                servicio.Estado = "Registrado";

                _context.ServiciosFunerarios.Add(servicio);
                await _context.SaveChangesAsync();

                return servicio.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar el servicio: {ex.Message}", ex);
            }
        }

        // CU-006: Obtener servicio por ID
        public async Task<ServicioFunerario> ObtenerPorIdAsync(int id)
        {
            try
            {
                return await _context.ServiciosFunerarios
                    .FirstOrDefaultAsync(s => s.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el servicio: {ex.Message}", ex);
            }
        }

        // CU-009: Obtener por código de expediente
        public async Task<ServicioFunerario> ObtenerPorExpedienteAsync(string codigoExpediente)
        {
            try
            {
                return await _context.ServiciosFunerarios
                    .FirstOrDefaultAsync(s => s.CodigoExpediente == codigoExpediente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar el expediente: {ex.Message}", ex);
            }
        }

        // CU-006, CU-009: Listar con filtros
        public async Task<List<ServicioFunerario>> ListarAsync(string buscar = null,
            string estado = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null,
            string paquete = null)
        {
            try
            {
                var query = _context.ServiciosFunerarios.AsQueryable();

                // Filtro por búsqueda (nombre difunto o expediente)
                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    query = query.Where(s =>
                        s.NombreDifunto.Contains(buscar) ||
                        s.CodigoExpediente.Contains(buscar));
                }

                // Filtro por estado
                if (!string.IsNullOrWhiteSpace(estado))
                {
                    query = query.Where(s => s.Estado == estado);
                }

                // Filtro por fecha desde
                if (fechaDesde.HasValue)
                {
                    query = query.Where(s => s.FechaRegistro >= fechaDesde.Value);
                }

                // Filtro por fecha hasta
                if (fechaHasta.HasValue)
                {
                    query = query.Where(s => s.FechaRegistro <= fechaHasta.Value);
                }

                // Filtro por paquete
                if (!string.IsNullOrWhiteSpace(paquete))
                {
                    query = query.Where(s => s.Paquete == paquete);
                }

                return await query
                    .OrderByDescending(s => s.FechaRegistro)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al listar servicios: {ex.Message}", ex);
            }
        }

        // CU-007: Actualizar estado
        public async Task ActualizarEstadoAsync(int id, string nuevoEstado, string observaciones)
        {
            try
            {
                var servicio = await ObtenerPorIdAsync(id);

                if (servicio == null)
                {
                    throw new Exception($"No se encontró el servicio con ID {id}");
                }

                servicio.Estado = nuevoEstado;

                if (!string.IsNullOrWhiteSpace(observaciones))
                {
                    servicio.Observaciones = observaciones;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar estado: {ex.Message}", ex);
            }
        }

        // Actualizar servicio completo
        public async Task ActualizarAsync(ServicioFunerario servicio)
        {
            try
            {
                _context.ServiciosFunerarios.Update(servicio);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar servicio: {ex.Message}", ex);
            }
        }

        // CU-008: Cancelar servicio
        public async Task CancelarAsync(int id)
        {
            try
            {
                await ActualizarEstadoAsync(id, "Cancelado", "Servicio cancelado");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cancelar servicio: {ex.Message}", ex);
            }
        }

        // Para dashboard: contar servicios activos
        public async Task<int> ContarServiciosActivosAsync()
        {
            try
            {
                return await _context.ServiciosFunerarios
                    .Where(s => s.Estado == "Registrado" || s.Estado == "EnProceso")
                    .CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al contar servicios activos: {ex.Message}", ex);
            }
        }

        // Para dashboard: obtener servicios recientes
        public async Task<List<ServicioFunerario>> ObtenerRecientesAsync(int cantidad)
        {
            try
            {
                return await _context.ServiciosFunerarios
                    .OrderByDescending(s => s.FechaRegistro)
                    .Take(cantidad)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener servicios recientes: {ex.Message}", ex);
            }
        }

        // Método privado para generar código de expediente único
        private async Task<string> GenerarCodigoExpedienteAsync()
        {
            var año = DateTime.Now.Year;
            var ultimoServicio = await _context.ServiciosFunerarios
                .Where(s => s.CodigoExpediente.StartsWith($"EXP-{año}"))
                .OrderByDescending(s => s.CodigoExpediente)
                .FirstOrDefaultAsync();

            int numeroConsecutivo = 1;

            if (ultimoServicio != null)
            {
                // Extraer el número del último expediente (EXP-2025-001 -> 001)
                var partes = ultimoServicio.CodigoExpediente.Split('-');
                if (partes.Length == 3 && int.TryParse(partes[2], out int numero))
                {
                    numeroConsecutivo = numero + 1;
                }
            }

            return $"EXP-{año}-{numeroConsecutivo:D3}";
        }
    }
}
