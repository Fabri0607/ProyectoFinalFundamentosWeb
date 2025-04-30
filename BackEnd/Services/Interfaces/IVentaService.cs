using BackEnd.DTO;

namespace BackEnd.Services.Interfaces
{
    public interface IVentaService
    {

        List<VentaDTO> GetVentas();
        VentaDTO GetVentaById(int id);
        VentaDTO AddVenta(VentaDTO parametro);
        VentaDTO UpdateVenta(VentaDTO parametro);
        VentaDTO DeleteVenta(int id);
    }
}
