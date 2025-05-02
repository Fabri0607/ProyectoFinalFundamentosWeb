using BackEnd.DTO;

namespace BackEnd.Services.Interfaces
{
    public interface IVentaService
    {
        // Método actualizado para usar el nuevo DTO de creación
        VentaDTO ProcesarVenta(CrearVentaDTO crearVentaDTO);
        List<VentaDTO> GetVentas();
        VentaDTO GetVentaById(int id);
        List<VentaDTO> GetVentasByFecha(DateTime fechaInicio, DateTime fechaFin);
    }
}