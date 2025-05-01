using BackEnd.DTO;

namespace BackEnd.Services.Interfaces
{
    public interface IReporteService
    {
        ReporteInventarioDTO GenerarReporteInventario();
        ReporteVentasDTO GenerarReporteVentas(DateTime fechaInicio, DateTime fechaFin);
    }
}