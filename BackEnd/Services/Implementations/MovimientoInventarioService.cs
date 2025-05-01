using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using Entities.Entities;
using DAL.Interfaces;

namespace BackEnd.Services.Implementations
{
    public class MovimientoInventarioService : IMovimientoInventarioService
    {

        IUnidadDeTrabajo unidadDeTrabajo;

        public MovimientoInventarioService(IUnidadDeTrabajo unidadDeTrabajo)
        {
            this.unidadDeTrabajo = unidadDeTrabajo;
        }

        MovimientoInventarioDTO Convertir(MovimientoInventario movimientoInventario)
        {
            var result = new MovimientoInventarioDTO
            {
                MovimientoId = movimientoInventario.MovimientoId,
                Cantidad = movimientoInventario.Cantidad,
                FechaMovimiento = movimientoInventario.FechaMovimiento,
                Notas = movimientoInventario.Notas,
                ProductoId = movimientoInventario.ProductoId,
                TipoMovimientoId = movimientoInventario.TipoMovimientoId,
                Referencia = movimientoInventario.Referencia
            };
            var productoEntity = unidadDeTrabajo.ProductoDAL.FindById(result.ProductoId);
            if (productoEntity != null)
            {
                result.Producto = new ProductoDTO
                {
                    ProductoId = productoEntity.ProductoId,
                    Descripcion = productoEntity.Descripcion,
                    Stock = productoEntity.Stock,
                    StockMinimo = productoEntity.StockMinimo,
                    FechaCreacion = productoEntity.FechaCreacion,
                    FechaModificacion = productoEntity.FechaModificacion,
                    Activo = productoEntity.Activo,
                    Codigo = productoEntity.Codigo,
                    Nombre = productoEntity.Nombre,
                    PrecioCompra = productoEntity.PrecioCompra,
                    PrecioVenta = productoEntity.PrecioVenta
                };
            }
            return result;
        }

        MovimientoInventario Convertir(MovimientoInventarioDTO movimientoInventarioDTO)
        {
            return new MovimientoInventario
            {
                MovimientoId = movimientoInventarioDTO.MovimientoId,
                Cantidad = movimientoInventarioDTO.Cantidad,
                FechaMovimiento = movimientoInventarioDTO.FechaMovimiento,
                Notas = movimientoInventarioDTO.Notas,
                ProductoId = movimientoInventarioDTO.ProductoId,
                TipoMovimientoId = movimientoInventarioDTO.TipoMovimientoId,
                Referencia = movimientoInventarioDTO.Referencia
            };
        }

        public MovimientoInventarioDTO Add(MovimientoInventarioDTO movimientoInventarioDTO)
        {
            var movimientoInventario = Convertir(movimientoInventarioDTO);
            movimientoInventarioDTO.MovimientoId = movimientoInventario.MovimientoId;
            movimientoInventarioDTO.FechaMovimiento = movimientoInventario.FechaMovimiento;
            movimientoInventarioDTO.FechaMovimiento = DateTime.Now;
            unidadDeTrabajo.MovimientoInventarioDAL.Add(movimientoInventario);
            unidadDeTrabajo.Complete();
            return movimientoInventarioDTO;
        }

        public MovimientoInventarioDTO Delete(int movimientoInventarioId)
        {
            var movimientoInventario = unidadDeTrabajo.MovimientoInventarioDAL.FindById(movimientoInventarioId);
            unidadDeTrabajo.MovimientoInventarioDAL.Remove(movimientoInventario);
            unidadDeTrabajo.Complete();
            return new MovimientoInventarioDTO { MovimientoId = movimientoInventarioId };
        }

        public MovimientoInventarioDTO Get(int movimientoInventarioId)
        {
            var movimientoInventario = unidadDeTrabajo.MovimientoInventarioDAL.FindById(movimientoInventarioId);
            return Convertir(movimientoInventario);
        }

        public List<MovimientoInventarioDTO> GetAll()
        {
            var movimientoInventarios = unidadDeTrabajo.MovimientoInventarioDAL.Get();
            var lista = new List<MovimientoInventarioDTO>();

            foreach (var item in movimientoInventarios)
            {
                lista.Add(Convertir(item));
            }

            return lista;
        }

        public MovimientoInventarioDTO Update(MovimientoInventarioDTO movimientoInventarioDTO)
        {
            var movimientoInventario = Convertir(movimientoInventarioDTO);
            movimientoInventario.FechaMovimiento = DateTime.Now;
            unidadDeTrabajo.MovimientoInventarioDAL.Update(movimientoInventario);
            unidadDeTrabajo.Complete();
            return movimientoInventarioDTO;
        }
    }
}
