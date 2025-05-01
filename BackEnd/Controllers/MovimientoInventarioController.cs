using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoInventarioController : ControllerBase
    {

        IMovimientoInventarioService _movimientoInventarioService;

        // GET: api/<MovimientoInventarioController>
        [HttpGet]
        public IEnumerable<MovimientoInventarioDTO> Get()
        {
           var result = _movimientoInventarioService.GetAll();
            return result;
        }

        // GET api/<MovimientoInventarioController>/5
        [HttpGet("{id}")]
        public MovimientoInventarioDTO Get(int id)
        {
            var result = _movimientoInventarioService.Get(id);
            return result;
        }

        // POST api/<MovimientoInventarioController>
        [HttpPost]
        public void Post([FromBody] MovimientoInventarioDTO value)
        {
            _movimientoInventarioService.Add(value);
        }

        // PUT api/<MovimientoInventarioController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] MovimientoInventarioDTO value)
        {
            _movimientoInventarioService.Update(value);
        }

        // DELETE api/<MovimientoInventarioController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _movimientoInventarioService.Delete(id);
        }
    }
}
