namespace BackEnd.DTO
{
    public class DetalleVentaDTO
    {
        public int DetalleVentaId { get; set; }

        public int Cantidad { get; set; }

        public int VentaId { get; set; }

        public int ProductoId { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Descuento { get; set; }

        public ProductoDTO? Producto { get; set; }

        public VentaDTO? Venta { get; set; }
    }
}