using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using DAL.Implementations;
using DAL.Interfaces;
using Entities.Entities;

namespace BackEnd.Services.Implementations
{
    public class ProductoService : IProductoService
    {
        ILogger<ParametroService> _logger;
        IUnidadDeTrabajo unidadDeTrabajo;
        private readonly IMovimientoInventarioService _movimientoService;

        public ProductoService(IUnidadDeTrabajo unidadDeTrabajo, ILogger<ParametroService> logger, IMovimientoInventarioService movimientoService)
        {
            this.unidadDeTrabajo = unidadDeTrabajo;
            _logger = logger;
            _movimientoService = movimientoService;
        }

        ParametroDTO Convertir(Parametro parametro)
        {
            return new ParametroDTO
            {
                ParametroId = parametro.ParametroId,
                Descripcion = parametro.Descripcion,
                Activo = parametro.Activo,
                Tipo = parametro.Tipo,
                Codigo = parametro.Codigo,
                Valor = parametro.Valor
            };
        }

        ProductoDTO Convertir(Producto producto)
        {
            var result = new ProductoDTO
            {
                ProductoId = producto.ProductoId,
                Descripcion = producto.Descripcion,
                Stock = producto.Stock,
                StockMinimo = producto.StockMinimo,
                FechaCreacion = producto.FechaCreacion,
                FechaModificacion = producto.FechaModificacion,
                Activo = producto.Activo,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                PrecioCompra = producto.PrecioCompra,
                PrecioVenta = producto.PrecioVenta,
                CategoriaId = producto.CategoriaId
            };

            var parametroEntity = unidadDeTrabajo.ParametroDAL.FindById(result.CategoriaId);

            if (parametroEntity != null)
            {
                result.Parametro = Convertir(parametroEntity);
            }

            return result;
        }

        Producto Convertir(ProductoDTO producto)
        {
            return new Producto
            {
                ProductoId = producto.ProductoId,
                Descripcion = producto.Descripcion,
                Stock = producto.Stock,
                StockMinimo = producto.StockMinimo,
                FechaCreacion = producto.FechaCreacion,
                FechaModificacion = producto.FechaModificacion,
                Activo = producto.Activo,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                PrecioCompra = producto.PrecioCompra,
                PrecioVenta = producto.PrecioVenta,
                CategoriaId = producto.CategoriaId
            };
        }

        public ProductoDTO Add(ProductoDTO productoDTO)
        {
            var producto = this.Convertir(productoDTO);

            producto.ProductoId = 0; // <- te aseguras que vaya 0
            producto.FechaCreacion = DateTime.Now; // <- fecha actual
            producto.FechaModificacion = null; // <- no hay modificación aún

            unidadDeTrabajo.ProductoDAL.Add(producto);

            unidadDeTrabajo.Complete();

            var movimiento = new MovimientoInventarioDTO
            {
                ProductoId = producto.ProductoId,
                Cantidad = producto.Stock,
                FechaMovimiento = DateTime.Now,
                TipoMovimientoId = 4, // Entrada
                Notas = $"Descripcion: {producto.Descripcion}",
                Referencia = producto.CategoriaId.ToString()
            };

            _movimientoService.AddMovimiento(movimiento);

            unidadDeTrabajo.Complete();
            return productoDTO;
        }



        public ProductoDTO Delete(int ProductoId)
        {
            try
            {
                _logger.LogInformation($"Iniciando eliminación del producto con ID {ProductoId}");

                // Verificar si el producto existe
                var producto = unidadDeTrabajo.ProductoDAL.FindById(ProductoId);
                if (producto == null)
                {
                    _logger.LogError($"Producto no encontrado: ID {ProductoId}");
                    throw new Exception($"No existe el producto con ID {ProductoId}");
                }

                // Obtener y eliminar todos los movimientos de inventario asociados
                var movimientos = unidadDeTrabajo.MovimientoInventarioDAL.Get()
                    .Where(m => m.ProductoId == ProductoId)
                    .ToList();

                foreach (var movimiento in movimientos)
                {
                    unidadDeTrabajo.MovimientoInventarioDAL.Remove(movimiento);
                    _logger.LogInformation($"Eliminado movimiento ID {movimiento.MovimientoId} asociado al producto ID {ProductoId}");
                }

                // Eliminar el producto
                unidadDeTrabajo.ProductoDAL.Remove(producto);
                _logger.LogInformation($"Producto ID {ProductoId} eliminado");

                // Guardar todos los cambios en una transacción
                unidadDeTrabajo.Complete();

                _logger.LogInformation($"Eliminación completada para el producto ID {ProductoId}");
                return new ProductoDTO { ProductoId = ProductoId };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el producto ID {ProductoId}: {ex.Message}");
                throw;
            }
        }

        public ProductoDTO Get(int ProductoId)
        {
            var producto = unidadDeTrabajo.ProductoDAL.FindById(ProductoId);
            return Convertir(producto);
        }

        public List<ProductoDTO> GetAll()
        {
            var productos = unidadDeTrabajo.ProductoDAL.Get();
            var list = new List<ProductoDTO>();

            foreach (var item in productos)
            {
                list.Add(Convertir(item));
            }

            return list;
        }

        public ProductoDTO Update(ProductoDTO productoDTO)
        {
            var producto = Convertir(productoDTO);

            producto.FechaModificacion = DateTime.Now; // <- actualizas la fecha de modificación

            unidadDeTrabajo.ProductoDAL.Update(producto);
            unidadDeTrabajo.Complete();

            var movimiento = new MovimientoInventarioDTO
            {
                ProductoId = producto.ProductoId,
                Cantidad = producto.Stock,
                FechaMovimiento = DateTime.Now,
                TipoMovimientoId = 6, // Ajuste
                Notas = $"Descripcion: {producto.Descripcion}",
                Referencia = producto.CategoriaId.ToString()
            };

            _movimientoService.AddMovimiento(movimiento);

            unidadDeTrabajo.Complete();
            return productoDTO;
        }

    }
}

