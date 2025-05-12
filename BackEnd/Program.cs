using System.Text;
using BackEnd.Services.Implementations;
using BackEnd.Services.Interfaces;
using DAL.Implementations;
using DAL.Interfaces;
using Entities.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .AllowAnyOrigin() // En producción, especificar el origen exacto: .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

#region DB

builder.Services.AddDbContext<SistemaInventarioVentasContext> (
                                options =>
                                options.UseSqlServer(
                                    builder
                                    .Configuration
                                    .GetConnectionString("DefaultConnection")
                                        ));

builder.Services.AddDbContext<AuthDBContext>(
                                options =>
                                options.UseSqlServer(
                                    builder
                                    .Configuration
                                    .GetConnectionString("DefaultConnection")
                                        ));

#endregion

#region Identity
builder.Services.AddIdentityCore<IdentityUser>()
                        .AddRoles<IdentityRole>()
                        .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("UNA")
                        .AddEntityFrameworkStores<AuthDBContext>()
                        .AddDefaultTokenProviders();



builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

});




#endregion


#region JWT

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding
                                                        .UTF8
                                                        .GetBytes(builder.Configuration["JWT:Key"]))

        };
    });






#endregion



#region DI
builder.Services.AddDbContext<SistemaInventarioVentasContext>();

builder.Services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();

builder.Services.AddScoped<IParametroDAL, ParametroDALImpl>();
builder.Services.AddScoped<IParametroService, ParametroService>();

builder.Services.AddScoped<IProductoDAL, ProductoDALImpl>();
builder.Services.AddScoped<IProductoService, ProductoService>();

builder.Services.AddScoped<IVentaDAL, VentaDALImpl>();
builder.Services.AddScoped<IVentaService, VentaService>();

builder.Services.AddScoped<IDetalleVentaDAL, DetalleVentaDALImpl>();
builder.Services.AddScoped<IDetalleVentaService, DetalleVentaService>();

builder.Services.AddScoped<IMovimientoInventarioDAL, MovimientoInventarioDALImpl>();
builder.Services.AddScoped<IMovimientoInventarioService, MovimientoInventarioService>();

builder.Services.AddScoped<IReporteService, ReporteService>();

builder.Services.AddScoped<ITokenService, TokenService>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usar CORS 
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();