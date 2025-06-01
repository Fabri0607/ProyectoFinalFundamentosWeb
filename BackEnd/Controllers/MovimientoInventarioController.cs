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
    public class MovimientoInventarioController : ControllerBase
    {
        private readonly IMovimientoInventarioService _movimientoService;
        private readonly ILogger<MovimientoInventarioController> _logger;

        public MovimientoInventarioController(
            IMovimientoInventarioService movimientoService,
            ILogger<MovimientoInventarioController> logger)
        {
            _movimientoService = movimientoService;
            _logger = logger;
        }

        // GET: api/MovimientoInventario
        [HttpGet]
        public ActionResult<IEnumerable<MovimientoInventarioDTO>> GetAll()
        {
            try
            {
                var movimientos = _movimientoService.GetAllMovimientos();
                return Ok(movimientos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener movimientos: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al obtener los movimientos");
            }
        }

        // GET: api/MovimientoInventario/5
        [HttpGet("{id}")]
        public ActionResult<MovimientoInventarioDTO> Get(int id)
        {
            try
            {
                var movimiento = _movimientoService.GetMovimientoById(id);
                return Ok(movimiento);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener movimiento {id}: {ex.Message}");
                return NotFound($"No se encontró el movimiento con ID {id}");
            }
        }

        // GET: api/MovimientoInventario/Producto/5
        [HttpGet("Producto/{productoId}")]
        public ActionResult<IEnumerable<MovimientoInventarioDTO>> GetByProducto(int productoId)
        {
            try
            {
                var movimientos = _movimientoService.GetMovimientosByProducto(productoId);
                return Ok(movimientos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener movimientos del producto {productoId}: {ex.Message}");
                return StatusCode(500, $"Error al obtener movimientos del producto {productoId}");
            }
        }

        // POST: api/MovimientoInventario
        [HttpPost]
        public ActionResult<MovimientoInventarioDTO> Post([FromBody] MovimientoInventarioDTO movimiento)
        {
            try
            {
                var resultado = _movimientoService.AddMovimiento(movimiento);
                return CreatedAtAction(nameof(Get), new { id = resultado.MovimientoId }, resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear movimiento: {ex.Message}");
                return BadRequest($"Error al crear el movimiento: {ex.Message}");
            }
        }

        // No implementamos PUT porque los movimientos de inventario normalmente no se modifican
        // Si se necesita corregir un movimiento, se debería realizar un nuevo movimiento en sentido opuesto
    }
}
