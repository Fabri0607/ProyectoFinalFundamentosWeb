using BackEnd.DTO;
using Entities.Entities;

namespace BackEnd.Services.Interfaces
{
    public interface IProductoService
    {
        ProductoDTO Add(ProductoDTO productoDTO);
        ProductoDTO Update(ProductoDTO productoDTO);
        ProductoDTO Delete(int ProductoId);
        ProductoDTO Get(int ProductoId);
        List<ProductoDTO> GetAll();

    }
}
