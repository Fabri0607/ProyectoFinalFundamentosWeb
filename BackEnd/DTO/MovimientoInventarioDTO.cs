using Entities.Entities;

namespace BackEnd.DTO
{
    public class MovimientoInventarioDTO
    {
        public int MovimientoId { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string? Notas { get; set; }
        public int ProductoId { get; set; }
        public int TipoMovimientoId { get; set; }
        public string? Referencia { get; set; }

        // Propiedades de navegación (opcionales para mostrar información relacionada)
        public string? ProductoNombre { get; set; }
        public string? TipoMovimientoNombre { get; set; }
    }
}
