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
            // Obtener el producto actual de la base de datos
            var productoActual = unidadDeTrabajo.ProductoDAL.FindById(productoDTO.ProductoId);
            if (productoActual == null)
            {
                throw new Exception("Producto no encontrado");
            }

            // Capturar los valores originales ANTES de hacer cualquier cambio
            var valoresOriginales = new
            {
                Nombre = productoActual.Nombre,
                Descripcion = productoActual.Descripcion,
                Stock = productoActual.Stock,
                StockMinimo = productoActual.StockMinimo,
                PrecioCompra = productoActual.PrecioCompra,
                PrecioVenta = productoActual.PrecioVenta,
                Codigo = productoActual.Codigo,
                CategoriaId = productoActual.CategoriaId,
                Activo = productoActual.Activo
            };

            // Generar la descripción de cambios ANTES de aplicar los cambios
            var cambios = new List<string>();

            // Comparar cada propiedad con los valores del DTO
            if (valoresOriginales.Nombre != productoDTO.Nombre)
            {
                cambios.Add($"Se editó el nombre de '{valoresOriginales.Nombre}' a '{productoDTO.Nombre}'");
            }

            if (valoresOriginales.Descripcion != productoDTO.Descripcion)
            {
                cambios.Add($"Se editó la descripción de '{valoresOriginales.Descripcion}' a '{productoDTO.Descripcion}'");
            }

            if (valoresOriginales.Stock != productoDTO.Stock)
            {
                cambios.Add($"Se editó el stock de {valoresOriginales.Stock} a {productoDTO.Stock}");
            }

            if (valoresOriginales.StockMinimo != productoDTO.StockMinimo)
            {
                cambios.Add($"Se editó el stock mínimo de {valoresOriginales.StockMinimo} a {productoDTO.StockMinimo}");
            }

            if (valoresOriginales.PrecioCompra != productoDTO.PrecioCompra)
            {
                cambios.Add($"Se editó el precio de compra de ₡{valoresOriginales.PrecioCompra} a ₡{productoDTO.PrecioCompra}");
            }

            if (valoresOriginales.PrecioVenta != productoDTO.PrecioVenta)
            {
                cambios.Add($"Se editó el precio de venta de ₡{valoresOriginales.PrecioVenta} a ₡{productoDTO.PrecioVenta}");
            }

            if (valoresOriginales.Codigo != productoDTO.Codigo)
            {
                cambios.Add($"Se editó el código de '{valoresOriginales.Codigo}' a '{productoDTO.Codigo}'");
            }

            if (valoresOriginales.CategoriaId != productoDTO.CategoriaId)
            {
                // Aquí podrías obtener los nombres de las categorías si tienes acceso al repositorio de categorías
                cambios.Add($"Se editó la categoría de ID {valoresOriginales.CategoriaId} a ID {productoDTO.CategoriaId}");
            }

            if (valoresOriginales.Activo != productoDTO.Activo)
            {
                string estadoOriginal = valoresOriginales.Activo ? "Activo" : "Inactivo";
                string estadoNuevo = productoDTO.Activo ? "Activo" : "Inactivo";
                cambios.Add($"Se editó el estado de '{estadoOriginal}' a '{estadoNuevo}'");
            }

            // AHORA aplicar los cambios al producto
            productoActual.Nombre = productoDTO.Nombre;
            productoActual.Descripcion = productoDTO.Descripcion;
            productoActual.Stock = productoDTO.Stock;
            productoActual.StockMinimo = productoDTO.StockMinimo;
            productoActual.PrecioCompra = productoDTO.PrecioCompra;
            productoActual.PrecioVenta = productoDTO.PrecioVenta;
            productoActual.Codigo = productoDTO.Codigo;
            productoActual.CategoriaId = productoDTO.CategoriaId;
            productoActual.Activo = productoDTO.Activo;
            productoActual.FechaModificacion = DateTime.Now;

            // Crear la descripción de cambios
            string descripcionCambios = cambios.Count > 0
                ? $"Descripción: {string.Join(", ", cambios)}"
                : "Descripción: No se realizaron cambios";

            // Actualizar el producto (no necesitas llamar Update explícitamente ya que el producto está siendo trackeado)
            unidadDeTrabajo.Complete();

            // Crear el movimiento con la descripción de cambios
            var movimiento = new MovimientoInventarioDTO
            {
                ProductoId = productoActual.ProductoId,
                Cantidad = productoActual.Stock,
                FechaMovimiento = DateTime.Now,
                TipoMovimientoId = 6, // Ajuste
                Notas = descripcionCambios,
                Referencia = productoActual.CategoriaId.ToString()
            };

            _movimientoService.AddMovimiento(movimiento);
            unidadDeTrabajo.Complete();

            return productoDTO;
        }

    }
}

