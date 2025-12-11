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
    /// Implementación del repositorio de inventario
    /// Maneja el acceso a datos para inventario (CU-014 a CU-018)
    /// </summary>
    public class InventarioRepository : IInventarioRepository
    {
        private readonly FunerariaDbContext _context;

        public InventarioRepository(FunerariaDbContext context)
        {
            _context = context;
        }

        // CU-014: Consultar inventario
        public async Task<List<ArticuloInventario>> ConsultarInventarioAsync(
            string categoria = null, string estadoStock = null, string buscar = null)
        {
            try
            {
                var query = _context.Inventario.AsQueryable();

                // Filtro por categoría
                if (!string.IsNullOrWhiteSpace(categoria))
                {
                    query = query.Where(a => a.Categoria == categoria);
                }

                // Filtro por estado de stock
                if (!string.IsNullOrWhiteSpace(estadoStock))
                {
                    query = query.Where(a => a.EstadoStock == estadoStock);
                }

                // Filtro por búsqueda
                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    query = query.Where(a =>
                        a.Nombre.Contains(buscar) ||
                        a.Codigo.Contains(buscar));
                }

                var articulos = await query
                    .OrderBy(a => a.Categoria)
                    .ThenBy(a => a.Nombre)
                    .ToListAsync();

                // Actualizar estado de stock para cada artículo
                foreach (var articulo in articulos)
                {
                    ActualizarEstadoStock(articulo);
                }

                await _context.SaveChangesAsync();

                return articulos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al consultar inventario: {ex.Message}", ex);
            }
        }

        public async Task<ArticuloInventario> ObtenerPorIdAsync(int id)
        {
            try
            {
                var articulo = await _context.Inventario
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (articulo != null)
                {
                    ActualizarEstadoStock(articulo);
                    await _context.SaveChangesAsync();
                }

                return articulo;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener artículo: {ex.Message}", ex);
            }
        }

        // CU-015: Registrar entrada de inventario
        public async Task RegistrarEntradaAsync(int articuloId, int cantidad,
            string motivo, string observaciones)
        {
            try
            {
                var articulo = await ObtenerPorIdAsync(articuloId);

                if (articulo == null)
                {
                    throw new Exception($"No se encontró el artículo con ID {articuloId}");
                }

                if (cantidad <= 0)
                {
                    throw new Exception("La cantidad debe ser mayor a cero");
                }

                articulo.CantidadDisponible += cantidad;
                articulo.FechaActualizacion = DateTime.Now;
                ActualizarEstadoStock(articulo);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar entrada: {ex.Message}", ex);
            }
        }

        // CU-016: Registrar salida de inventario
        public async Task RegistrarSalidaAsync(int articuloId, int cantidad,
            string motivo, string observaciones)
        {
            try
            {
                var articulo = await ObtenerPorIdAsync(articuloId);

                if (articulo == null)
                {
                    throw new Exception($"No se encontró el artículo con ID {articuloId}");
                }

                if (cantidad <= 0)
                {
                    throw new Exception("La cantidad debe ser mayor a cero");
                }

                if (articulo.CantidadDisponible < cantidad)
                {
                    throw new Exception($"Stock insuficiente. Disponible: {articulo.CantidadDisponible}");
                }

                articulo.CantidadDisponible -= cantidad;
                articulo.FechaActualizacion = DateTime.Now;
                ActualizarEstadoStock(articulo);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar salida: {ex.Message}", ex);
            }
        }

        // CU-018: Actualizar artículo
        public async Task ActualizarArticuloAsync(ArticuloInventario articulo)
        {
            try
            {
                articulo.FechaActualizacion = DateTime.Now;
                ActualizarEstadoStock(articulo);

                _context.Inventario.Update(articulo);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar artículo: {ex.Message}", ex);
            }
        }

        // Consultar inventario crítico
        public async Task<List<ArticuloInventario>> ConsultarInventarioCriticoAsync()
        {
            try
            {
                var articulos = await _context.Inventario
                    .Where(a => a.CantidadDisponible <= a.StockMinimo)
                    .OrderBy(a => a.CantidadDisponible)
                    .ToListAsync();

                foreach (var articulo in articulos)
                {
                    ActualizarEstadoStock(articulo);
                }

                await _context.SaveChangesAsync();

                return articulos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al consultar inventario crítico: {ex.Message}", ex);
            }
        }

        // Para dashboard: contar artículos críticos
        public async Task<int> ContarArticulosCriticosAsync()
        {
            try
            {
                return await _context.Inventario
                    .Where(a => a.CantidadDisponible <= a.StockMinimo)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al contar artículos críticos: {ex.Message}", ex);
            }
        }

        // CU-017: Generar alertas de stock bajo
        public async Task<List<ArticuloInventario>> GenerarAlertasStockBajoAsync()
        {
            try
            {
                return await ConsultarInventarioCriticoAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar alertas: {ex.Message}", ex);
            }
        }

        // Método privado para actualizar estado de stock (RN-013, RN-014)
        private void ActualizarEstadoStock(ArticuloInventario articulo)
        {
            if (articulo.CantidadDisponible == 0)
            {
                articulo.EstadoStock = "Agotado";
            }
            else if (articulo.CantidadDisponible <= articulo.StockMinimo)
            {
                articulo.EstadoStock = "Critico"; // RN-013
            }
            else if (articulo.CantidadDisponible <= articulo.StockMinimo * 1.5)
            {
                articulo.EstadoStock = "Bajo";
            }
            else
            {
                articulo.EstadoStock = "Normal";
            }
        }
    }
}
