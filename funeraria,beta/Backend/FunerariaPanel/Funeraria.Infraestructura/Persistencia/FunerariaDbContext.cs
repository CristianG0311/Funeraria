using Funeraria.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Infraestructura.Persistencia
{
    /// <summary>
    /// Contexto de base de datos para el sistema de funeraria
    /// Maneja todas las operaciones con Entity Framework Core
    /// </summary>
    public class FunerariaDbContext : DbContext
    {
        public FunerariaDbContext(DbContextOptions<FunerariaDbContext> options)
            : base(options)
        {
        }

        public DbSet<ArticuloInventario> ArticulosInventario { get; set; }
        // DbSets - Representan las tablas de la base de datos
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ServicioFunerario> ServiciosFunerarios { get; set; }
        public DbSet<DocumentoLegal> DocumentosLegales { get; set; }
        public DbSet<ArticuloInventario> Inventario { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar las entidades usando Fluent API
            ConfigurarUsuarios(modelBuilder);
            ConfigurarServiciosFunerarios(modelBuilder);
            ConfigurarDocumentosLegales(modelBuilder);
            ConfigurarInventario(modelBuilder);
            ConfigurarFacturas(modelBuilder);
            ConfigurarPagos(modelBuilder);
        }

        private void ConfigurarUsuarios(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.NombreUsuario)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(e => e.NombreUsuario)
                    .IsUnique();

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.NombreCompleto)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .HasMaxLength(100);

                entity.Property(e => e.Rol)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");

                // Datos iniciales (seed data)
                entity.HasData(new Usuario
                {
                    Id = 1,
                    NombreUsuario = "admin",
                    PasswordHash = "Admin123",
                    NombreCompleto = "Administrador del Sistema",
                    Email = "admin@funeraria.com",
                    Rol = "Administrador",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                });
            });
        }

        private void ConfigurarServiciosFunerarios(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServicioFunerario>(entity =>
            {
                entity.ToTable("ServiciosFunerarios");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.CodigoExpediente)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(e => e.CodigoExpediente)
                    .IsUnique();

                entity.Property(e => e.NombreDifunto)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CedulaDifunto)
                    .HasMaxLength(20);

                entity.Property(e => e.FechaFallecimiento)
                    .IsRequired();

                entity.Property(e => e.LugarFallecimiento)
                    .HasMaxLength(100);

                entity.Property(e => e.NombreFamiliar)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Parentesco)
                    .HasMaxLength(50);

                entity.Property(e => e.TelefonoFamiliar)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.EmailFamiliar)
                    .HasMaxLength(100);

                entity.Property(e => e.Paquete)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.TipoServicio)
                    .HasMaxLength(20);

                entity.Property(e => e.SalaVelacion)
                    .HasMaxLength(20);

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Registrado");

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(500);

                // Índices para búsquedas
                entity.HasIndex(e => e.Estado);
                entity.HasIndex(e => e.FechaRegistro);
            });
        }

        private void ConfigurarDocumentosLegales(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentoLegal>(entity =>
            {
                entity.ToTable("DocumentosLegales");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TipoDocumento)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NumeroDocumento)
                    .HasMaxLength(50);

                entity.Property(e => e.EntidadEmisora)
                    .HasMaxLength(100);

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Pendiente");

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(500);

                entity.Property(e => e.RutaArchivo)
                    .HasMaxLength(255);

                entity.Property(e => e.EsObligatorio)
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaActualizacion)
                    .HasDefaultValueSql("GETDATE()");

                // Índices
                entity.HasIndex(e => e.ServicioId);
                entity.HasIndex(e => e.Estado);
                entity.HasOne(d => d.Servicio)
           .WithMany()
           .HasForeignKey(d => d.ServicioId)
           .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigurarInventario(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticuloInventario>(entity =>
            {
                entity.ToTable("Inventario");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Codigo)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(e => e.Codigo)
                    .IsUnique();

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500);

                entity.Property(e => e.Categoria)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.EstadoStock)
                    .HasMaxLength(20);

                entity.Property(e => e.PrecioVenta)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.PrecioCosto)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.FechaActualizacion)
                    .HasDefaultValueSql("GETDATE()");

                // Índices
                entity.HasIndex(e => e.Categoria);
                entity.HasIndex(e => e.EstadoStock);

                // Datos iniciales (seed data)
                entity.HasData(
                    new ArticuloInventario
                    {
                        Id = 1,
                        Codigo = "ATAUD-001",
                        Nombre = "Ataúd Premium Caoba",
                        Descripcion = "Ataúd de madera de caoba con acabado premium y herrajes dorados",
                        Categoria = "Ataudes",
                        CantidadDisponible = 5,
                        StockMinimo = 2,
                        EstadoStock = "Normal",
                        PrecioVenta = 2500.00m,
                        PrecioCosto = 1800.00m,
                        FechaActualizacion = DateTime.Now
                    },
                    new ArticuloInventario
                    {
                        Id = 2,
                        Codigo = "ATAUD-002",
                        Nombre = "Ataúd Básico Pino",
                        Descripcion = "Ataúd económico de madera de pino",
                        Categoria = "Ataudes",
                        CantidadDisponible = 8,
                        StockMinimo = 3,
                        EstadoStock = "Normal",
                        PrecioVenta = 800.00m,
                        PrecioCosto = 500.00m,
                        FechaActualizacion = DateTime.Now
                    },
                    new ArticuloInventario
                    {
                        Id = 3,
                        Codigo = "URNA-001",
                        Nombre = "Urna de Porcelana Blanca",
                        Descripcion = "Urna elegante de porcelana con detalles dorados",
                        Categoria = "Urnas",
                        CantidadDisponible = 12,
                        StockMinimo = 5,
                        EstadoStock = "Normal",
                        PrecioVenta = 250.00m,
                        PrecioCosto = 150.00m,
                        FechaActualizacion = DateTime.Now
                    },
                    new ArticuloInventario
                    {
                        Id = 4,
                        Codigo = "URNA-002",
                        Nombre = "Urna de Madera",
                        Descripcion = "Urna de madera tallada a mano",
                        Categoria = "Urnas",
                        CantidadDisponible = 3,
                        StockMinimo = 3,
                        EstadoStock = "Critico",
                        PrecioVenta = 180.00m,
                        PrecioCosto = 120.00m,
                        FechaActualizacion = DateTime.Now
                    },
                    new ArticuloInventario
                    {
                        Id = 5,
                        Codigo = "FLOR-001",
                        Nombre = "Ramo de Rosas Blancas",
                        Descripcion = "Arreglo de 24 rosas blancas naturales",
                        Categoria = "Flores",
                        CantidadDisponible = 25,
                        StockMinimo = 10,
                        EstadoStock = "Normal",
                        PrecioVenta = 80.00m,
                        PrecioCosto = 45.00m,
                        FechaActualizacion = DateTime.Now
                    },
                    new ArticuloInventario
                    {
                        Id = 6,
                        Codigo = "FLOR-002",
                        Nombre = "Corona Fúnebre Grande",
                        Descripcion = "Corona circular de flores mixtas con moño negro",
                        Categoria = "Flores",
                        CantidadDisponible = 15,
                        StockMinimo = 8,
                        EstadoStock = "Normal",
                        PrecioVenta = 150.00m,
                        PrecioCosto = 90.00m,
                        FechaActualizacion = DateTime.Now
                    },
                    new ArticuloInventario
                    {
                        Id = 7,
                        Codigo = "FLOR-003",
                        Nombre = "Cruz de Flores",
                        Descripcion = "Cruz de 1.5m de altura con flores blancas",
                        Categoria = "Flores",
                        CantidadDisponible = 6,
                        StockMinimo = 4,
                        EstadoStock = "Bajo",
                        PrecioVenta = 200.00m,
                        PrecioCosto = 120.00m,
                        FechaActualizacion = DateTime.Now
                    },
                    new ArticuloInventario
                    {
                        Id = 8,
                        Codigo = "VELA-001",
                        Nombre = "Velas Grandes 48 horas",
                        Descripcion = "Set de 4 velas blancas de larga duración",
                        Categoria = "Velas",
                        CantidadDisponible = 50,
                        StockMinimo = 20,
                        EstadoStock = "Normal",
                        PrecioVenta = 15.00m,
                        PrecioCosto = 8.00m,
                        FechaActualizacion = DateTime.Now
                    },
                    new ArticuloInventario
                    {
                        Id = 9,
                        Codigo = "REL-001",
                        Nombre = "Libro de Condolencias",
                        Descripcion = "Libro elegante para firmas de visitantes",
                        Categoria = "Articulos Religiosos",
                        CantidadDisponible = 30,
                        StockMinimo = 10,
                        EstadoStock = "Normal",
                        PrecioVenta = 25.00m,
                        PrecioCosto = 12.00m,
                        FechaActualizacion = DateTime.Now
                    },
                    new ArticuloInventario
                    {
                        Id = 10,
                        Codigo = "VEST-001",
                        Nombre = "Mortaja Blanca Adulto",
                        Descripcion = "Mortaja de algodón blanco talla estándar",
                        Categoria = "Vestimenta",
                        CantidadDisponible = 20,
                        StockMinimo = 10,
                        EstadoStock = "Normal",
                        PrecioVenta = 60.00m,
                        PrecioCosto = 35.00m,
                        FechaActualizacion = DateTime.Now
                    },
                    new ArticuloInventario
                    {
                        Id = 11,
                        Codigo = "VEST-002",
                        Nombre = "Traje Formal Hombre",
                        Descripcion = "Traje negro completo con corbata",
                        Categoria = "Vestimenta",
                        CantidadDisponible = 8,
                        StockMinimo = 5,
                        EstadoStock = "Normal",
                        PrecioVenta = 120.00m,
                        PrecioCosto = 70.00m,
                        FechaActualizacion = DateTime.Now
                    },
                    new ArticuloInventario
                    {
                        Id = 12,
                        Codigo = "COSM-001",
                        Nombre = "Kit de Maquillaje Mortuorio",
                        Descripcion = "Set completo de cosméticos para preparación",
                        Categoria = "Cosmeticos",
                        CantidadDisponible = 10,
                        StockMinimo = 5,
                        EstadoStock = "Normal",
                        PrecioVenta = 85.00m,
                        PrecioCosto = 50.00m,
                        FechaActualizacion = DateTime.Now
                    },
                    new ArticuloInventario
                    {
                        Id = 13,
                        Codigo = "LIB-001",
                        Nombre = "Libro de Condolencias Premium",
                        Descripcion = "Libro de firmas con portada de cuero sintético negro, 80 páginas con líneas doradas",
                        Categoria = "Articulos Religiosos",
                        CantidadDisponible = 15,
                        StockMinimo = 8,
                        EstadoStock = "Normal",
                        PrecioVenta = 35.00m,
                        PrecioCosto = 18.00m,
                        FechaActualizacion = DateTime.Now
                    }
                );
            });
        }

        private void ConfigurarFacturas(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Factura>(entity =>
            {
                entity.ToTable("Facturas");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.NumeroFactura)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(e => e.NumeroFactura)
                    .IsUnique();

                entity.Property(e => e.FechaEmision)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.Subtotal)
                    .HasColumnType("decimal(10,2)")

                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Itbms)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Descuento)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValue(0);

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.MontoPagado)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValue(0);

                entity.Property(e => e.SaldoPendiente)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.MetodoPago)
                    .HasMaxLength(50);

                entity.Property(e => e.RutaPdf)
                    .HasMaxLength(255);

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(500);

                // Índices
                entity.HasIndex(e => e.ServicioId);
                entity.HasIndex(e => e.Pagada);
            });
        }

        private void ConfigurarPagos(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pago>(entity =>
            {
                entity.ToTable("Pagos");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Monto)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.FechaPago)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.MetodoPago)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NumeroReferencia)
                    .HasMaxLength(50);

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(500);

                // Índices
                entity.HasIndex(e => e.FacturaId);
                entity.HasIndex(e => e.FechaPago);
            });
        }
    }
}

