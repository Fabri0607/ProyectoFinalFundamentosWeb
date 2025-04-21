using BackEnd.Services.Implementations;
using BackEnd.Services.Interfaces;
using DAL.Implementations;
using DAL.Interfaces;
using Entities.Entities;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Serilog

builder.Logging.ClearProviders();
builder.Host.UseSerilog((ctx, lc) => lc
                        .WriteTo
                        .File("logs/logsbackend", rollingInterval: RollingInterval.Day)
                        .MinimumLevel.Error());




#endregion


#region DI
builder.Services.AddDbContext<SistemaInventarioVentasContext>();

builder.Services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();

builder.Services.AddScoped<IParametroDAL, ParametroDALImpl>();
builder.Services.AddScoped<IParametroService, ParametroService>();

builder.Services.AddScoped<IProductoDAL, ProductoDALImpl>();
builder.Services.AddScoped<IProductoService, ProductoService>();

builder.Services.AddScoped<IVentaDAL, VentaDALImpl>();


builder.Services.AddScoped<IDetalleVentaDAL, DetalleVentaDALImpl>();


builder.Services.AddScoped<IMovimientoInventarioDAL, MovimientoInventarioDALImpl>();



#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
