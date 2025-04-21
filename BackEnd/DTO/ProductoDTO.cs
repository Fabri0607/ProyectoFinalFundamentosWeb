namespace BackEnd.DTO
{
    public class ProductoDTO
    {
        public int ProductoId { get; set; }

        public string Descripcion { get; set; } = null!;

        public int Stock { get; set; }

        public int StockMinimo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public bool Activo { get; set; }

        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public decimal PrecioCompra { get; set; }

        public decimal PrecioVenta { get; set; }

        public int CategoriaId { get; set; }

        public ParametroDTO? Parametro { get; set; }
    }
}
