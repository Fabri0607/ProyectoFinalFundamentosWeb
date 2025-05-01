using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        // GET: api/<VentaController>
        [HttpGet]
        public IEnumerable<VentaDTO> Get()
        {
            return _ventaService.GetVentas();
        }

        // GET api/<VentaController>/5
        [HttpGet("{id}")]
        public VentaDTO Get(int id)
        {
            return _ventaService.GetVentaById(id);
        }

        // POST api/<VentaController>
        [HttpPost]
        public void Post([FromBody] VentaDTO venta)
        {
            _ventaService.AddVenta(venta);
        }

        // PUT api/<VentaController>/5
        [HttpPut]
        public void Put([FromBody] VentaDTO venta)
        {
            _ventaService.UpdateVenta(venta);
        }

        // DELETE api/<VentaController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _ventaService.DeleteVenta(id);
        }
    }
}
