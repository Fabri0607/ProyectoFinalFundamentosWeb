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

        public  ProductoDTO? Producto { get; set; } = null!;

        public  ParametroDTO? TipoMovimiento { get; set; } = null!;
    }
}
