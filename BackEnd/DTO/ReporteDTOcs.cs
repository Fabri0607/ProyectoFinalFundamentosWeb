namespace BackEnd.DTO
{
    #region Reportes de Inventario

    public class ReporteInventarioDTO
    {
        public DateTime FechaGeneracion { get; set; }
        public int TotalProductos { get; set; }
        public decimal ValorizacionTotal { get; set; }
        public List<CategoriaInventarioDTO> ProductosPorCategoria { get; set; } = new List<CategoriaInventarioDTO>();
        public List<ProductoStockBajoDTO> ProductosStockBajo { get; set; } = new List<ProductoStockBajoDTO>();
        public List<ProductoSinMovimientoDTO> ProductosSinMovimiento { get; set; } = new List<ProductoSinMovimientoDTO>();
    }

    public class CategoriaInventarioDTO
    {
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = null!;
        public int CantidadProductos { get; set; }
        public decimal ValorizacionTotal { get; set; }
    }

    public class ProductoStockBajoDTO
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public int DiferenciaBajo { get; set; }
    }

    public class ProductoSinMovimientoDTO
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public int Stock { get; set; }
        public DateTime? UltimoMovimiento { get; set; }
    }

    #endregion

    #region Reportes de Ventas

    public class ReporteVentasDTO
    {
        public DateTime FechaGeneracion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TotalVentas { get; set; }
        public decimal MontoTotalVentas { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal UtilidadTotal { get; set; }
        public List<VentasPorDiaDTO> VentasPorDia { get; set; } = new List<VentasPorDiaDTO>();
        public List<ProductoVentaDTO> ProductosMasVendidos { get; set; } = new List<ProductoVentaDTO>();
        public List<VentasPorMetodoPagoDTO> VentasPorMetodoPago { get; set; } = new List<VentasPorMetodoPagoDTO>();
    }

    public class VentasPorDiaDTO
    {
        public DateTime Fecha { get; set; }
        public int CantidadVentas { get; set; }
        public decimal TotalVentas { get; set; }
    }

    public class ProductoVentaDTO
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public int CantidadVendida { get; set; }
        public decimal TotalVentas { get; set; }
    }

    public class VentasPorMetodoPagoDTO
    {
        public string MetodoPago { get; set; } = null!;
        public int CantidadVentas { get; set; }
        public decimal TotalVentas { get; set; }
    }

    #endregion
}