using BackEnd.DTO;
using BackEnd.Exceptions;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Authorize(Roles = "Admin,Colaborador,Vendedor")]
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;
        private readonly ILogger<VentaController> _logger;

        public VentaController(
            IVentaService ventaService,
            ILogger<VentaController> logger)
        {
            _ventaService = ventaService;
            _logger = logger;
        }

        // GET: api/Venta
        [HttpGet]
        public ActionResult<IEnumerable<VentaDTO>> Get()
        {
            try
            {
                var ventas = _ventaService.GetVentas();
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener ventas: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al obtener las ventas");
            }
        }

        // GET: api/Venta/5
        [HttpGet("{id}")]
        public ActionResult<VentaDTO> Get(int id)
        {
            try
            {
                var venta = _ventaService.GetVentaById(id);
                return Ok(venta);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener venta {id}: {ex.Message}");
                return NotFound($"No se encontró la venta con ID {id}");
            }
        }

        // GET: api/Venta/Periodo?fechaInicio=2023-01-01&fechaFin=2023-01-31
        [HttpGet("Periodo")]
        public ActionResult<IEnumerable<VentaDTO>> GetPorPeriodo(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                var ventas = _ventaService.GetVentasByFecha(fechaInicio, fechaFin);
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener ventas por periodo: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al obtener las ventas por periodo");
            }
        }

        // POST: api/Venta
        [HttpPost]
        public ActionResult<VentaDTO> Post([FromBody] CrearVentaDTO crearVenta)
        {
            try
            {
                var resultado = _ventaService.ProcesarVenta(crearVenta);
                return CreatedAtAction(nameof(Get), new { id = resultado.VentaId }, resultado);
            }
            catch (StockInsuficienteException ex)
            {
                _logger.LogError($"Error de stock al procesar venta: {ex.Message}");
                return BadRequest(new
                {
                    Error = "StockInsuficiente",
                    Message = ex.Message,
                    Details = new
                    {
                        ex.ProductoNombre,
                        ex.StockDisponible,
                        ex.CantidadSolicitada
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al procesar venta: {ex.Message}");
                return BadRequest(new
                {
                    Error = "General",
                    Message = $"Error al procesar la venta: {ex.Message}"
                });
            }
        }
    }
}