using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public IActionResult Delete(int id)
        {
            try
            {
                _parametroService.DeleteParametro(id);
                return NoContent();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
            {
                return Conflict(new { message = "No se puede eliminar el parámetro porque está asociado a productos o movimientos" });
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Conflict(new { message = "No se puede eliminar el parámetro porque está asociado a productos o movimientos" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error interno al eliminar el parámetro" });
            }
        }
    }
}

