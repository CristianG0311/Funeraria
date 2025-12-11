using Funeraria.Aplicacion.DTOs.Inventario;
using Funeraria.Aplicacion.Interfaces;
using Funeraria.Dominio.Entidades;
using Funeraria.Dominio.Interfaces;
using Funeraria.Infraestructura.Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Aplicacion.Servicios
{
    /// <summary>
    /// Servicio de aplicación para inventario
    /// Implementa los casos de uso CU-014 a CU-018
    /// </summary>
    public class InventarioService : IInventarioService
    {
        private readonly IInventarioRepository _inventarioRepository;

        private readonly FunerariaDbContext _dbContext; // usa FunerariaDbContext

        public InventarioService(IInventarioRepository inventarioRepository, FunerariaDbContext dbContext)
        {
            _inventarioRepository = inventarioRepository;
            _dbContext = dbContext;
        }



        // CU-014: Consultar inventario
        public async Task<List<ArticuloInventarioDto>> ConsultarInventarioAsync(
            string categoria = null, string estadoStock = null, string buscar = null)
        {
            var articulos = await _inventarioRepository.ConsultarInventarioAsync(
                categoria, estadoStock, buscar);

            return articulos.Select(a => new ArticuloInventarioDto
            {
                Id = a.Id,
                Codigo = a.Codigo,
                Nombre = a.Nombre,
                Descripcion = a.Descripcion,
                Categoria = a.Categoria,
                CantidadDisponible = a.CantidadDisponible,
                StockMinimo = a.StockMinimo,
                EstadoStock = a.EstadoStock,
                PrecioVenta = a.PrecioVenta,
                PrecioCosto = a.PrecioCosto,
                FechaActualizacion = a.FechaActualizacion
            }).ToList();
        }

        // CU-015: Registrar entrada de inventario
        public async Task RegistrarEntradaAsync(MovimientoInventarioRequest request)
        {
            // Validaciones
            ValidarMovimiento(request);

            if (request.TipoMovimiento.ToLower() != "entrada")
            {
                throw new ArgumentException("El tipo de movimiento debe ser 'Entrada'");
            }

            await _inventarioRepository.RegistrarEntradaAsync(
                request.ArticuloId,
                request.Cantidad,
                request.Motivo,
                request.Observaciones
            );
        }

        // CU-016: Registrar salida de inventario
        public async Task RegistrarSalidaAsync(MovimientoInventarioRequest request)
        {
            // Validaciones
            ValidarMovimiento(request);

            if (request.TipoMovimiento.ToLower() != "salida")
            {
                throw new ArgumentException("El tipo de movimiento debe ser 'Salida'");
            }

            await _inventarioRepository.RegistrarSalidaAsync(
                request.ArticuloId,
                request.Cantidad,
                request.Motivo,
                request.Observaciones
            );
        }

        // CU-017: Generar alertas de stock bajo
        public async Task<List<ArticuloInventarioDto>> GenerarAlertasStockBajoAsync()
        {
            var articulos = await _inventarioRepository.GenerarAlertasStockBajoAsync();

            return articulos.Select(a => new ArticuloInventarioDto
            {
                Id = a.Id,
                Codigo = a.Codigo,
                Nombre = a.Nombre,
                Descripcion = a.Descripcion,
                Categoria = a.Categoria,
                CantidadDisponible = a.CantidadDisponible,
                StockMinimo = a.StockMinimo,
                EstadoStock = a.EstadoStock,
                PrecioVenta = a.PrecioVenta,
                PrecioCosto = a.PrecioCosto,
                FechaActualizacion = a.FechaActualizacion
            }).ToList();
        }

        // CU-018: Actualizar artículo de inventario
        public async Task ActualizarArticuloAsync(int id, ArticuloInventarioDto articulo)
        {
            // Validar que el artículo exista
            var articuloExistente = await _inventarioRepository.ObtenerPorIdAsync(id);

            if (articuloExistente == null)
            {
                throw new Exception($"No se encontró el artículo con ID {id}");
            }

            // Validaciones
            if (string.IsNullOrWhiteSpace(articulo.Nombre))
            {
                throw new ArgumentException("El nombre del artículo es obligatorio");
            }

            if (articulo.PrecioVenta < 0)
            {
                throw new ArgumentException("El precio de venta no puede ser negativo");
            }

            if (articulo.PrecioCosto < 0)
            {
                throw new ArgumentException("El precio de costo no puede ser negativo");
            }

            if (articulo.StockMinimo < 0)
            {
                throw new ArgumentException("El stock mínimo no puede ser negativo");
            }

            // Actualizar
            articuloExistente.Nombre = articulo.Nombre;
            articuloExistente.Descripcion = articulo.Descripcion;
            articuloExistente.Categoria = articulo.Categoria;
            articuloExistente.StockMinimo = articulo.StockMinimo;
            articuloExistente.PrecioVenta = articulo.PrecioVenta;
            articuloExistente.PrecioCosto = articulo.PrecioCosto;

            await _inventarioRepository.ActualizarArticuloAsync(articuloExistente);
        }

        // Método privado de validación
        private void ValidarMovimiento(MovimientoInventarioRequest request)
        {
            if (request.ArticuloId <= 0)
            {
                throw new ArgumentException("El ID del artículo es inválido");
            }

            if (request.Cantidad <= 0)
            {
                throw new ArgumentException("La cantidad debe ser mayor a cero");
            }

            if (string.IsNullOrWhiteSpace(request.Motivo))
            {
                throw new ArgumentException("El motivo es obligatorio");
            }
        }

        public async Task<ArticuloInventarioDto> CrearArticuloAsync(CrearArticuloRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Codigo) || string.IsNullOrWhiteSpace(request.Nombre))
                throw new ArgumentException("Código y nombre son obligatorios");

            var articulo = new ArticuloInventario
            {
                Codigo = request.Codigo,
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Categoria = request.Categoria,
                CantidadDisponible = request.CantidadDisponible,
                StockMinimo = request.StockMinimo,
                PrecioCosto = request.PrecioCosto,
                PrecioVenta = request.PrecioVenta,
                EstadoStock = CalcularEstadoStock(request.CantidadDisponible, request.StockMinimo),
                FechaActualizacion = DateTime.UtcNow
                // ❌ Ubicacion eliminado
            };

            _dbContext.ArticulosInventario.Add(articulo);
            await _dbContext.SaveChangesAsync();

            return new ArticuloInventarioDto
            {
                Id = articulo.Id,
                Codigo = articulo.Codigo,
                Nombre = articulo.Nombre,
                Descripcion = articulo.Descripcion,
                Categoria = articulo.Categoria,
                CantidadDisponible = articulo.CantidadDisponible,
                StockMinimo = articulo.StockMinimo,
                PrecioCosto = articulo.PrecioCosto,
                PrecioVenta = articulo.PrecioVenta,
                EstadoStock = articulo.EstadoStock,
                FechaActualizacion = articulo.FechaActualizacion
                // ❌ Ubicacion eliminado
            };
        }

        private string CalcularEstadoStock(int cantidad, int minimo)
        {
            if (cantidad <= 0) return "Agotado";
            if (cantidad <= minimo) return "Crítico";
            if (cantidad <= minimo + 2) return "Bajo";
            return "Normal";
        }

    }
}
