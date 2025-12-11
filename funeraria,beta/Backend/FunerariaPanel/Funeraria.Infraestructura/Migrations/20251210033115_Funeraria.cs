using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Funeraria.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class Funeraria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Facturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicioId = table.Column<int>(type: "int", nullable: false),
                    NumeroFactura = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Itbms = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Pagada = table.Column<bool>(type: "bit", nullable: false),
                    MontoPagado = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    SaldoPendiente = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RutaPdf = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inventario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CantidadDisponible = table.Column<int>(type: "int", nullable: false),
                    StockMinimo = table.Column<int>(type: "int", nullable: false),
                    EstadoStock = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PrecioVenta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PrecioCosto = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacturaId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    MetodoPago = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroReferencia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiciosFunerarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoExpediente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NombreDifunto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CedulaDifunto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaFallecimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Edad = table.Column<int>(type: "int", nullable: false),
                    LugarFallecimiento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NombreFamiliar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Parentesco = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TelefonoFamiliar = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EmailFamiliar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Paquete = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoServicio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SalaVelacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CeremoniaReligiosa = table.Column<bool>(type: "bit", nullable: false),
                    GestionDocumentalCompleta = table.Column<bool>(type: "bit", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Registrado"),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosFunerarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosLegales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicioId = table.Column<int>(type: "int", nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EntidadEmisora = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EsObligatorio = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosLegales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentosLegales_ServiciosFunerarios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "ServiciosFunerarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Inventario",
                columns: new[] { "Id", "CantidadDisponible", "Categoria", "Codigo", "Descripcion", "EstadoStock", "FechaActualizacion", "Nombre", "PrecioCosto", "PrecioVenta", "StockMinimo" },
                values: new object[,]
                {
                    { 1, 5, "Ataudes", "ATAUD-001", "Ataúd de madera de caoba con acabado premium y herrajes dorados", "Normal", new DateTime(2025, 12, 9, 22, 31, 15, 470, DateTimeKind.Local).AddTicks(8376), "Ataúd Premium Caoba", 1800.00m, 2500.00m, 2 },
                    { 2, 8, "Ataudes", "ATAUD-002", "Ataúd económico de madera de pino", "Normal", new DateTime(2025, 12, 9, 22, 31, 15, 470, DateTimeKind.Local).AddTicks(8386), "Ataúd Básico Pino", 500.00m, 800.00m, 3 },
                    { 3, 12, "Urnas", "URNA-001", "Urna elegante de porcelana con detalles dorados", "Normal", new DateTime(2025, 12, 9, 22, 31, 15, 470, DateTimeKind.Local).AddTicks(8388), "Urna de Porcelana Blanca", 150.00m, 250.00m, 5 },
                    { 4, 3, "Urnas", "URNA-002", "Urna de madera tallada a mano", "Critico", new DateTime(2025, 12, 9, 22, 31, 15, 470, DateTimeKind.Local).AddTicks(8390), "Urna de Madera", 120.00m, 180.00m, 3 },
                    { 5, 25, "Flores", "FLOR-001", "Arreglo de 24 rosas blancas naturales", "Normal", new DateTime(2025, 12, 9, 22, 31, 15, 470, DateTimeKind.Local).AddTicks(8391), "Ramo de Rosas Blancas", 45.00m, 80.00m, 10 },
                    { 6, 15, "Flores", "FLOR-002", "Corona circular de flores mixtas con moño negro", "Normal", new DateTime(2025, 12, 9, 22, 31, 15, 470, DateTimeKind.Local).AddTicks(8393), "Corona Fúnebre Grande", 90.00m, 150.00m, 8 },
                    { 7, 6, "Flores", "FLOR-003", "Cruz de 1.5m de altura con flores blancas", "Bajo", new DateTime(2025, 12, 9, 22, 31, 15, 470, DateTimeKind.Local).AddTicks(8395), "Cruz de Flores", 120.00m, 200.00m, 4 },
                    { 8, 50, "Velas", "VELA-001", "Set de 4 velas blancas de larga duración", "Normal", new DateTime(2025, 12, 9, 22, 31, 15, 470, DateTimeKind.Local).AddTicks(8396), "Velas Grandes 48 horas", 8.00m, 15.00m, 20 },
                    { 9, 30, "Articulos Religiosos", "REL-001", "Libro elegante para firmas de visitantes", "Normal", new DateTime(2025, 12, 9, 22, 31, 15, 470, DateTimeKind.Local).AddTicks(8398), "Libro de Condolencias", 12.00m, 25.00m, 10 },
                    { 10, 20, "Vestimenta", "VEST-001", "Mortaja de algodón blanco talla estándar", "Normal", new DateTime(2025, 12, 9, 22, 31, 15, 470, DateTimeKind.Local).AddTicks(8400), "Mortaja Blanca Adulto", 35.00m, 60.00m, 10 },
                    { 11, 8, "Vestimenta", "VEST-002", "Traje negro completo con corbata", "Normal", new DateTime(2025, 12, 9, 22, 31, 15, 470, DateTimeKind.Local).AddTicks(8402), "Traje Formal Hombre", 70.00m, 120.00m, 5 },
                    { 12, 10, "Cosmeticos", "COSM-001", "Set completo de cosméticos para preparación", "Normal", new DateTime(2025, 12, 9, 22, 31, 15, 470, DateTimeKind.Local).AddTicks(8404), "Kit de Maquillaje Mortuorio", 50.00m, 85.00m, 5 },
                    { 13, 15, "Articulos Religiosos", "LIB-001", "Libro de firmas con portada de cuero sintético negro, 80 páginas con líneas doradas", "Normal", new DateTime(2025, 12, 9, 22, 31, 15, 470, DateTimeKind.Local).AddTicks(8405), "Libro de Condolencias Premium", 18.00m, 35.00m, 8 }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Email", "FechaCreacion", "NombreCompleto", "NombreUsuario", "PasswordHash", "Rol", "UltimoAcceso" },
                values: new object[] { 1, true, "admin@funeraria.com", new DateTime(2025, 12, 9, 22, 31, 15, 470, DateTimeKind.Local).AddTicks(856), "Administrador del Sistema", "admin", "Admin123", "Administrador", null });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosLegales_Estado",
                table: "DocumentosLegales",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosLegales_ServicioId",
                table: "DocumentosLegales",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_NumeroFactura",
                table: "Facturas",
                column: "NumeroFactura",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_Pagada",
                table: "Facturas",
                column: "Pagada");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_ServicioId",
                table: "Facturas",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventario_Categoria",
                table: "Inventario",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Inventario_Codigo",
                table: "Inventario",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventario_EstadoStock",
                table: "Inventario",
                column: "EstadoStock");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_FacturaId",
                table: "Pagos",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_FechaPago",
                table: "Pagos",
                column: "FechaPago");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosFunerarios_CodigoExpediente",
                table: "ServiciosFunerarios",
                column: "CodigoExpediente",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosFunerarios_Estado",
                table: "ServiciosFunerarios",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosFunerarios_FechaRegistro",
                table: "ServiciosFunerarios",
                column: "FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentosLegales");

            migrationBuilder.DropTable(
                name: "Facturas");

            migrationBuilder.DropTable(
                name: "Inventario");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "ServiciosFunerarios");
        }
    }
}
