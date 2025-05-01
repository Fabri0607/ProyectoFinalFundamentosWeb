using BackEnd.DTO;

namespace BackEnd.Services.Interfaces
{
    public interface IDetalleVentaService
    {
        DetalleVentaDTO Add(DetalleVentaDTO detalleVentaDTO);
        DetalleVentaDTO Update(DetalleVentaDTO detalleVentaDTO);
        DetalleVentaDTO Delete(int detalleVentaId);
        DetalleVentaDTO Get(int detalleVentaId);
        List<DetalleVentaDTO> GetAll();
        List<DetalleVentaDTO> GetByVenta(int ventaId);
    }
}