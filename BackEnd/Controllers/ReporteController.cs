using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Authorize(Roles = "Admin,Colaborador,Vendedor")]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _reporteService;
        private readonly ILogger<ReporteController> _logger;

        public ReporteController(
            IReporteService reporteService,
            ILogger<ReporteController> logger)
        {
            _reporteService = reporteService;
            _logger = logger;
        }

        // GET: api/Reporte/Inventario
        [HttpGet("Inventario")]
        public ActionResult<ReporteInventarioDTO> GetReporteInventario()
        {
            try
            {
                _logger.LogInformation("Iniciando generación de reporte de inventario");

                var reporte = _reporteService.GenerarReporteInventario();

                _logger.LogInformation($"Reporte generado exitosamente. Total productos: {reporte.TotalProductos}, Valorización: {reporte.ValorizacionTotal}");

                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de inventario");
                return StatusCode(500, new
                {
                    message = "Error al generar el reporte de inventario",
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        // GET: api/Reporte/Ventas?fechaInicio=2023-01-01&fechaFin=2023-01-31
        [HttpGet("Ventas")]
        public ActionResult<ReporteVentasDTO> GetReporteVentas(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                // Validar fechas
                if (fechaInicio > fechaFin)
                {
                    return BadRequest("La fecha de inicio debe ser anterior a la fecha final");
                }

                // Si no se especifica fecha final, usar la fecha actual
                if (fechaFin == default)
                {
                    fechaFin = DateTime.Now;
                }

                _logger.LogInformation($"Generando reporte de ventas del {fechaInicio:yyyy-MM-dd} al {fechaFin:yyyy-MM-dd}");

                var reporte = _reporteService.GenerarReporteVentas(fechaInicio, fechaFin);

                _logger.LogInformation($"Reporte de ventas generado. Total ventas: {reporte.TotalVentas}, Monto total: {reporte.MontoTotalVentas}");

                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de ventas");
                return StatusCode(500, new
                {
                    message = "Error al generar el reporte de ventas",
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        // GET: api/Reporte/StockBajo
        [HttpGet("StockBajo")]
        public ActionResult<IEnumerable<ProductoStockBajoDTO>> GetProductosStockBajo()
        {
            try
            {
                _logger.LogInformation("Obteniendo productos con stock bajo");

                var reporte = _reporteService.GenerarReporteInventario();

                _logger.LogInformation($"Productos con stock bajo encontrados: {reporte.ProductosStockBajo.Count}");

                return Ok(reporte.ProductosStockBajo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos con stock bajo");
                return StatusCode(500, new
                {
                    message = "Error al obtener productos con stock bajo",
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        // GET: api/Reporte/ProductosMasVendidos?fechaInicio=2023-01-01&fechaFin=2023-01-31
        [HttpGet("ProductosMasVendidos")]
        public ActionResult<IEnumerable<ProductoVentaDTO>> GetProductosMasVendidos(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                // Validar fechas
                if (fechaInicio > fechaFin)
                {
                    return BadRequest("La fecha de inicio debe ser anterior a la fecha final");
                }

                // Si no se especifica fecha final, usar la fecha actual
                if (fechaFin == default)
                {
                    fechaFin = DateTime.Now;
                }

                _logger.LogInformation($"Obteniendo productos más vendidos del {fechaInicio:yyyy-MM-dd} al {fechaFin:yyyy-MM-dd}");

                var reporte = _reporteService.GenerarReporteVentas(fechaInicio, fechaFin);

                _logger.LogInformation($"Productos más vendidos obtenidos: {reporte.ProductosMasVendidos.Count}");

                return Ok(reporte.ProductosMasVendidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos más vendidos");
                return StatusCode(500, new
                {
                    message = "Error al obtener productos más vendidos",
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        // GET: api/Reporte/ProductosSinMovimiento
        [HttpGet("ProductosSinMovimiento")]
        public ActionResult<IEnumerable<ProductoSinMovimientoDTO>> GetProductosSinMovimiento()
        {
            try
            {
                _logger.LogInformation("Obteniendo productos sin movimiento");

                var reporte = _reporteService.GenerarReporteInventario();

                _logger.LogInformation($"Productos sin movimiento encontrados: {reporte.ProductosSinMovimiento.Count}");

                return Ok(reporte.ProductosSinMovimiento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos sin movimiento");
                return StatusCode(500, new
                {
                    message = "Error al obtener productos sin movimiento",
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }
    }
}