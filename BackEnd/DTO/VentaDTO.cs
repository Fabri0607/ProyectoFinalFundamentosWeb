namespace BackEnd.DTO
{
    public class VentaDTO
    {
        public int VentaId { get; set; }

        public DateTime FechaVenta { get; set; }

        public string? Notas { get; set; }

        public string NumeroFactura { get; set; } = null!;

        public decimal Subtotal { get; set; }

        public decimal Impuestos { get; set; }

        public decimal Total { get; set; }

        public string MetodoPago { get; set; } = null!;

        public string EstadoVenta { get; set; } = null!;

        public List<DetalleVentaDTO> DetalleVenta { get; set; } = new List<DetalleVentaDTO>();
    }
}
