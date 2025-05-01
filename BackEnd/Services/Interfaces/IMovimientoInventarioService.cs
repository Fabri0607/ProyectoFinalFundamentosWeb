using BackEnd.DTO;
using Entities.Entities;

namespace BackEnd.Services.Interfaces
{
    public interface IMovimientoInventarioService
    {
        MovimientoInventarioDTO Add(MovimientoInventarioDTO movimientoInventarioDTO);
        MovimientoInventarioDTO Update(MovimientoInventarioDTO movimientoInventarioDTO);
        MovimientoInventarioDTO Delete(int movimientoInventarioId);
        MovimientoInventarioDTO Get(int movimientoInventarioId);
        List<MovimientoInventarioDTO> GetAll();
    }
}
