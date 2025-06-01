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
                var reporte = _reporteService.GenerarReporteInventario();
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al generar reporte de inventario: {ex.Message}");
                return StatusCode(500, "Error al generar el reporte de inventario");
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

                var reporte = _reporteService.GenerarReporteVentas(fechaInicio, fechaFin);
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al generar reporte de ventas: {ex.Message}");
                return StatusCode(500, "Error al generar el reporte de ventas");
            }
        }

        // GET: api/Reporte/StockBajo
        [HttpGet("StockBajo")]
        public ActionResult<IEnumerable<ProductoStockBajoDTO>> GetProductosStockBajo()
        {
            try
            {
                var reporte = _reporteService.GenerarReporteInventario();
                return Ok(reporte.ProductosStockBajo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener productos con stock bajo: {ex.Message}");
                return StatusCode(500, "Error al obtener productos con stock bajo");
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

                var reporte = _reporteService.GenerarReporteVentas(fechaInicio, fechaFin);
                return Ok(reporte.ProductosMasVendidos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener productos más vendidos: {ex.Message}");
                return StatusCode(500, "Error al obtener productos más vendidos");
            }
        }
    }
}