using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleVentaController : ControllerBase
    {
        IDetalleVentaService _detalleVentaService;

        public DetalleVentaController(IDetalleVentaService detalleVentaService)
        {
            this._detalleVentaService = detalleVentaService;
        }

        // GET: api/<DetalleVentaController>
        [HttpGet]
        public IEnumerable<DetalleVentaDTO> Get()
        {
            var result = _detalleVentaService.GetAll();
            return result;
        }

        // GET api/<DetalleVentaController>/5
        [HttpGet("{id}")]
        public DetalleVentaDTO Get(int id)
        {
            var result = _detalleVentaService.Get(id);
            return result;
        }

        // GET api/<DetalleVentaController>/venta/5
        [HttpGet("venta/{ventaId}")]
        public IEnumerable<DetalleVentaDTO> GetByVenta(int ventaId)
        {
            var result = _detalleVentaService.GetByVenta(ventaId);
            return result;
        }

        // POST api/<DetalleVentaController>
        [HttpPost]
        public void Post([FromBody] DetalleVentaDTO detalleVenta)
        {
            _detalleVentaService.Add(detalleVenta);
        }

        // PUT api/<DetalleVentaController>
        [HttpPut]
        public void Put([FromBody] DetalleVentaDTO detalleVenta)
        {
            _detalleVentaService.Update(detalleVenta);
        }

        // DELETE api/<DetalleVentaController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _detalleVentaService.Delete(id);
        }
    }
}