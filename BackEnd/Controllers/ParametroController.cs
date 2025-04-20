using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametroController : ControllerBase
    {
        IParametroService _parametroService;

        public ParametroController(IParametroService parametroService)
        {
            _parametroService = parametroService;
        }

        // GET: api/<ParametroController>
        [HttpGet]
        public IEnumerable<ParametroDTO> Get()
        {
            return _parametroService.GetParametros();
        }

        // GET api/<ParametroController>/5
        [HttpGet("{id}")]
        public ParametroDTO Get(int id)
        {
            return _parametroService.GetParametroById(id);
        }

        // POST api/<ParametroController>
        [HttpPost]
        public void Post([FromBody] ParametroDTO parametro)
        {
            _parametroService.AddParametro(parametro);
        }

        // PUT api/<ParametroController>/5
        [HttpPut]
        public void Put([FromBody] ParametroDTO parametro)
        {
            _parametroService.UpdateParametro(parametro);
        }

        // DELETE api/<ParametroController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _parametroService.DeleteParametro(id);
        }
    }
}

