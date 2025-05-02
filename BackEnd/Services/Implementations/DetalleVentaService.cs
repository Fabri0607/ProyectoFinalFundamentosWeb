using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using DAL.Interfaces;
using Entities.Entities;

namespace BackEnd.Services.Implementations
{
    public class DetalleVentaService : IDetalleVentaService
    {
        IUnidadDeTrabajo unidadDeTrabajo;

        public DetalleVentaService(IUnidadDeTrabajo unidadDeTrabajo)
        {
            this.unidadDeTrabajo = unidadDeTrabajo;
        }

        private ProductoDTO ConvertirProducto(Producto producto)
        {
            if (producto == null)
                return null;

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
                result.Parametro = ConvertirParametro(parametroEntity);
            }

            return result;
        }

        private ParametroDTO ConvertirParametro(Parametro parametro)
        {
            if (parametro == null)
                return null;

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

        private VentaDTO ConvertirVenta(Venta venta)
        {
            if (venta == null)
                return null;

            return new VentaDTO
            {
                VentaId = venta.VentaId,
                FechaVenta = venta.FechaVenta,
                Notas = venta.Notas,
                NumeroFactura = venta.NumeroFactura,
                Subtotal = venta.Subtotal,
                Impuestos = venta.Impuestos,
                Total = venta.Total,
                MetodoPago = venta.MetodoPago,
                EstadoVenta = venta.EstadoVenta
            };
        }

        private DetalleVentaDTO Convertir(DetalleVenta detalleVenta)
        {
            if (detalleVenta == null)
                return null;

            var result = new DetalleVentaDTO
            {
                DetalleVentaId = detalleVenta.DetalleVentaId,
                Cantidad = detalleVenta.Cantidad,
                VentaId = detalleVenta.VentaId,
                ProductoId = detalleVenta.ProductoId,
                PrecioUnitario = detalleVenta.PrecioUnitario,
                Subtotal = detalleVenta.Subtotal,
                Descuento = detalleVenta.Descuento
            };

            // Cargar el producto asociado si está disponible
            if (detalleVenta.Producto != null)
            {
                result.Producto = ConvertirProducto(detalleVenta.Producto);
            }
            else
            {
                var productoEntity = unidadDeTrabajo.ProductoDAL.FindById(detalleVenta.ProductoId);
                if (productoEntity != null)
                {
                    result.Producto = ConvertirProducto(productoEntity);
                }
            }

            // Cargar la venta asociada si está disponible
            if (detalleVenta.Venta != null)
            {
                result.Venta = ConvertirVenta(detalleVenta.Venta);
            }
            else
            {
                var ventaEntity = unidadDeTrabajo.VentaDAL.FindById(detalleVenta.VentaId);
                if (ventaEntity != null)
                {
                    result.Venta = ConvertirVenta(ventaEntity);
                }
            }

            return result;
        }

        private DetalleVenta Convertir(DetalleVentaDTO detalleVentaDTO)
        {
            if (detalleVentaDTO == null)
                return null;

            return new DetalleVenta
            {
                DetalleVentaId = detalleVentaDTO.DetalleVentaId,
                Cantidad = detalleVentaDTO.Cantidad,
                VentaId = detalleVentaDTO.VentaId,
                ProductoId = detalleVentaDTO.ProductoId,
                PrecioUnitario = detalleVentaDTO.PrecioUnitario,
                Subtotal = detalleVentaDTO.Subtotal,
                Descuento = detalleVentaDTO.Descuento
            };
        }

        public DetalleVentaDTO Add(DetalleVentaDTO detalleVentaDTO)
        {
            var detalleVenta = Convertir(detalleVentaDTO);

            // Asegurarse que sea un nuevo registro
            detalleVenta.DetalleVentaId = 0;

            // Actualizar el stock del producto (si es necesario en tu lógica de negocio)
            // Este es solo un ejemplo, ajústalo según tus requerimientos
            var producto = unidadDeTrabajo.ProductoDAL.FindById(detalleVenta.ProductoId);
            if (producto != null)
            {
                // Aquí podrías implementar lógica para verificar stock, etc.
                if (producto.Stock < detalleVenta.Cantidad)
                {
                    throw new InvalidOperationException("El stock del producto es insuficiente para realizar la venta.");
                }

                // Reducir el stock del producto
                producto.Stock -= detalleVenta.Cantidad;
                unidadDeTrabajo.ProductoDAL.Update(producto);
            }

            unidadDeTrabajo.DetalleVentaDAL.Add(detalleVenta);
            unidadDeTrabajo.Complete();

            return Convertir(detalleVenta);
        }

        public DetalleVentaDTO Delete(int detalleVentaId)
        {
            var detalleVenta = new DetalleVenta { DetalleVentaId = detalleVentaId };
            unidadDeTrabajo.DetalleVentaDAL.Remove(detalleVenta);
            unidadDeTrabajo.Complete();

            return new DetalleVentaDTO { DetalleVentaId = detalleVentaId };
        }

        public DetalleVentaDTO Get(int detalleVentaId)
        {
            var detalleVenta = unidadDeTrabajo.DetalleVentaDAL.FindById(detalleVentaId);
            return Convertir(detalleVenta);
        }

        public List<DetalleVentaDTO> GetAll()
        {
            var detallesVenta = unidadDeTrabajo.DetalleVentaDAL.Get();
            var list = new List<DetalleVentaDTO>();

            foreach (var item in detallesVenta)
            {
                list.Add(Convertir(item));
            }

            return list;
        }

        public List<DetalleVentaDTO> GetByVenta(int ventaId)
        {
            var detallesVenta = unidadDeTrabajo.DetalleVentaDAL.Get()
                .Where(d => d.VentaId == ventaId)
                .ToList();

            var list = new List<DetalleVentaDTO>();

            foreach (var item in detallesVenta)
            {
                list.Add(Convertir(item));
            }

            return list;
        }

        public DetalleVentaDTO Update(DetalleVentaDTO detalleVentaDTO)
        {
            var detalleVenta = Convertir(detalleVentaDTO);

            unidadDeTrabajo.DetalleVentaDAL.Update(detalleVenta);
            unidadDeTrabajo.Complete();

            return detalleVentaDTO;
        }
    }
}