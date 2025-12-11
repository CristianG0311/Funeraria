using Microsoft.EntityFrameworkCore;
using Funeraria.Infraestructura.Persistencia;
using Funeraria.Dominio.Interfaces;
using Funeraria.Infraestructura.Repositorios;
using Funeraria.Aplicacion.Interfaces;
using Funeraria.Aplicacion.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext con SQL Server y reintentos automáticos
builder.Services.AddDbContext<FunerariaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("FunerariaConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(60);
        }));

// Registrar repositorios
builder.Services.AddScoped<IServicioFunerarioRepository, ServicioFunerarioRepository>();
builder.Services.AddScoped<IDocumentoLegalRepository, DocumentoLegalRepository>();
builder.Services.AddScoped<IInventarioRepository, InventarioRepository>();
builder.Services.AddScoped<IFacturacionRepository, FacturacionRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Registrar servicios de aplicación
builder.Services.AddScoped<IServicioFunerarioService, ServicioFunerarioService>();
builder.Services.AddScoped<IDocumentoService, DocumentoService>();
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<IFacturacionService, FacturacionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Agregar controladores
// Agregar controladores con configuración JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Permite que las propiedades se mapeen sin importar mayúsculas/minúsculas
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

        // Fuerza a que el backend use camelCase al serializar respuestas
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });


// Configurar CORS para permitir llamadas desde el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger para documentación de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usar CORS
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
