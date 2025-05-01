using BackEnd.DTO;

namespace BackEnd.Services.Interfaces
{
    public interface IMovimientoInventarioService
    {
        MovimientoInventarioDTO AddMovimiento(MovimientoInventarioDTO movimientoDTO);
        List<MovimientoInventarioDTO> GetMovimientosByProducto(int productoId);
        List<MovimientoInventarioDTO> GetAllMovimientos();
        MovimientoInventarioDTO GetMovimientoById(int id);
    }
}
