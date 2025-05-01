using BackEnd.DTO;

namespace BackEnd.Services.Interfaces
{
    public interface IVentaService
    {

        List<VentaDTO> GetVentas();
        VentaDTO GetVentaById(int id);
        VentaDTO AddVenta(VentaDTO venta);
        VentaDTO UpdateVenta(VentaDTO venta);
        VentaDTO DeleteVenta(int id);
    }
}
