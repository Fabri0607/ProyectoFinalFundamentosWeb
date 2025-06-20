﻿using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Authorize(Roles = "Admin,Colaborador,Vendedor")]
    public class ProductoController : ControllerBase
    {
        IProductoService _productoService;
        IMovimientoInventarioService _movimientoService;

        public ProductoController(IProductoService productoService, IMovimientoInventarioService movimientoService)
        {
            this._productoService = productoService;
            _movimientoService = movimientoService;
        }

        // GET: api/<ProductoController>
        [HttpGet]
        
        public IEnumerable<ProductoDTO> Get()
        {
            var result = _productoService.GetAll();
            return result;
        }

        // GET api/<ProductoController>/5
        [HttpGet("{id}")]
        public ProductoDTO Get(int id)
        {
            var result = _productoService.Get(id);
            return result;
        }

        // POST api/<ProductoController>
        [HttpPost]
        public IActionResult Create([FromBody] ProductoDTO producto)
        {
            try
            {
                if (producto == null)
                {
                    return BadRequest("Los datos del producto son nulos.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _productoService.Add(producto);
                return  NoContent();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) // Violación de clave única
            {
                return Conflict(new { mensaje = $"Ya existe un producto con el código {producto.Codigo}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error al agregar el producto." });
            }
        }

        // PUT api/<ProductoController>
        [HttpPut]
        public void Put([FromBody] ProductoDTO producto)
        {
            _productoService.Update(producto);
        }

        // DELETE api/<ProductoController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _productoService.Delete(id);
                return NoContent();
            }
            catch (SqlException ex) when (ex.Number == 547) // 547 es el código para conflicto de clave foránea
            {
                return Conflict(new { message = "No se puede eliminar el producto porque tiene ventas asociadas" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "No se puede eliminar el producto porque tiene ventas asociadas" });
            }
        }
    }
}
