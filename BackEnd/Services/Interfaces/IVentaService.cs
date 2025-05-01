using BackEnd.DTO;

namespace BackEnd.Services.Interfaces
{
    public interface IVentaService
    {
        VentaDTO ProcesarVenta(VentaDTO ventaDTO);
        List<VentaDTO> GetVentas();
        VentaDTO GetVentaById(int id);
        List<VentaDTO> GetVentasByFecha(DateTime fechaInicio, DateTime fechaFin);
    }

}
