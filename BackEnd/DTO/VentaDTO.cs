namespace BackEnd.DTO
{
    public class VentaDTO
    {
        public int VentaId { get; set; }
        public DateTime FechaVenta { get; set; }
        public string? Notas { get; set; }
        public string NumeroFactura { get; set; } = null!; // Se generará automáticamente
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; } = "Efectivo"; // Valor por defecto
        public string EstadoVenta { get; set; } = null!; // Se generará automáticamente

        public List<DetalleVentaDTO> DetalleVenta { get; set; } = new List<DetalleVentaDTO>();
    }

    // Nuevo DTO específico para crear una venta (entrada)
    public class CrearVentaDTO
    {
        public string? Notas { get; set; }
        public string MetodoPago { get; set; } = "Efectivo"; // Valor por defecto

        public List<CrearDetalleVentaDTO> DetalleVenta { get; set; } = new List<CrearDetalleVentaDTO>();
    }

    // Nuevo DTO específico para los detalles en la creación
    public class CrearDetalleVentaDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Descuento { get; set; } = 0; // Valor por defecto
    }
}