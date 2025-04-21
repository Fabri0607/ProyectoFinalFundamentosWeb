using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using DAL.Interfaces;
using Entities.Entities;

namespace BackEnd.Services.Implementations
{
    public class ProductoService : IProductoService
    {
        IUnidadDeTrabajo unidadDeTrabajo;

        public ProductoService(IUnidadDeTrabajo unidadDeTrabajo)
        {
            this.unidadDeTrabajo = unidadDeTrabajo;
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
            return productoDTO;
        }



        public ProductoDTO Delete(int ProductoId)
        {
            var producto = new Producto { ProductoId = ProductoId };
            unidadDeTrabajo.ProductoDAL.Remove(producto);
            unidadDeTrabajo.Complete();
            return new ProductoDTO { ProductoId = ProductoId };
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

            return productoDTO;
        }

    }
}

