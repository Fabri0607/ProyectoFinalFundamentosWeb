using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using DAL.Interfaces;

namespace BackEnd.Services.Implementations
{
    public class ReporteService : IReporteService
    {
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly ILogger<ReporteService> _logger;

        public ReporteService(
            IUnidadDeTrabajo unidadDeTrabajo,
            ILogger<ReporteService> logger)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _logger = logger;
        }

        public ReporteInventarioDTO GenerarReporteInventario()
        {
            try
            {
                // Obtener todos los productos
                var productos = _unidadDeTrabajo.ProductoDAL.Get().ToList();

                // Calcular valorización total del inventario (usando precio de venta)
                decimal valorizacionTotal = productos.Sum(p => p.Stock * p.PrecioVenta);

                // Obtener todas las categorías/parámetros una sola vez
                var categorias = _unidadDeTrabajo.ParametroDAL.Get().ToList();

                // Contar productos por categoría
                var productosPorCategoria = productos
                    .GroupBy(p => p.CategoriaId)
                    .Select(g => {
                        var categoria = categorias.FirstOrDefault(c => c.ParametroId == g.Key);
                        return new CategoriaInventarioDTO
                        {
                            CategoriaId = g.Key,
                            CategoriaNombre = categoria?.Valor ?? "Sin Categoría",
                            CantidadProductos = g.Count(),
                            ValorizacionTotal = g.Sum(p => p.Stock * p.PrecioVenta)
                        };
                    })
                    .ToList();

                // Productos con stock bajo (menor o igual al mínimo)
                var productosStockBajo = productos
                    .Where(p => p.Stock <= p.StockMinimo)
                    .Select(p => new ProductoStockBajoDTO
                    {
                        ProductoId = p.ProductoId,
                        Nombre = p.Nombre,
                        Codigo = p.Codigo,
                        StockActual = p.Stock,
                        StockMinimo = p.StockMinimo,
                        DiferenciaBajo = p.StockMinimo - p.Stock
                    })
                    .OrderByDescending(p => p.DiferenciaBajo)
                    .ToList();

                // Productos sin movimiento en el último mes
                DateTime fechaLimite = DateTime.Now.AddMonths(-1);

                var movimientos = _unidadDeTrabajo.MovimientoInventarioDAL.Get().ToList();
                var productosIdConMovimiento = movimientos
                    .Where(m => m.FechaMovimiento >= fechaLimite)
                    .Select(m => m.ProductoId)
                    .Distinct()
                    .ToList();

                var productosSinMovimiento = productos
                    .Where(p => !productosIdConMovimiento.Contains(p.ProductoId))
                    .Select(p => {
                        var ultimoMovimiento = movimientos
                            .Where(m => m.ProductoId == p.ProductoId)
                            .OrderByDescending(m => m.FechaMovimiento)
                            .FirstOrDefault();

                        return new ProductoSinMovimientoDTO
                        {
                            ProductoId = p.ProductoId,
                            Nombre = p.Nombre,
                            Codigo = p.Codigo,
                            Stock = p.Stock,
                            UltimoMovimiento = ultimoMovimiento?.FechaMovimiento
                        };
                    })
                    .OrderBy(p => p.UltimoMovimiento)
                    .ToList();

                // Construir el reporte completo
                return new ReporteInventarioDTO
                {
                    FechaGeneracion = DateTime.Now,
                    TotalProductos = productos.Count(),
                    ValorizacionTotal = valorizacionTotal,
                    ProductosPorCategoria = productosPorCategoria,
                    ProductosStockBajo = productosStockBajo,
                    ProductosSinMovimiento = productosSinMovimiento
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al generar reporte de inventario: {ex.Message}");
                throw;
            }
        }

        public ReporteVentasDTO GenerarReporteVentas(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                // Obtener ventas en el período
                var ventas = _unidadDeTrabajo.VentaDAL.Get()
                    .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin)
                    .ToList();

                // Obtener detalles de ventas
                var detallesVenta = _unidadDeTrabajo.DetalleVentaDAL.Get()
                    .Where(d => ventas.Select(v => v.VentaId).Contains(d.VentaId))
                    .ToList();

                // Calcular totales
                decimal ventasTotal = ventas.Sum(v => v.Total);
                decimal costoTotal = 0;
                decimal utilidadTotal = 0;

                // Obtener productos una sola vez para optimizar
                var productos = _unidadDeTrabajo.ProductoDAL.Get().ToList();

                // Ventas por día
                var ventasPorDia = ventas
                    .GroupBy(v => v.FechaVenta.Date)
                    .Select(g => new VentasPorDiaDTO
                    {
                        Fecha = g.Key,
                        CantidadVentas = g.Count(),
                        TotalVentas = g.Sum(v => v.Total)
                    })
                    .OrderBy(v => v.Fecha)
                    .ToList();

                // Productos más vendidos
                var productosMasVendidos = detallesVenta
                    .GroupBy(d => d.ProductoId)
                    .Select(g => {
                        var producto = productos.FirstOrDefault(p => p.ProductoId == g.Key);
                        return new ProductoVentaDTO
                        {
                            ProductoId = g.Key,
                            Nombre = producto?.Nombre ?? "Producto no encontrado",
                            Codigo = producto?.Codigo ?? "",
                            CantidadVendida = g.Sum(d => d.Cantidad),
                            TotalVentas = g.Sum(d => d.Subtotal)
                        };
                    })
                    .OrderByDescending(p => p.CantidadVendida)
                    .Take(10)
                    .ToList();

                // Calcular costo y utilidad
                foreach (var detalle in detallesVenta)
                {
                    var producto = productos.FirstOrDefault(p => p.ProductoId == detalle.ProductoId);
                    if (producto != null)
                    {
                        decimal costoProducto = producto.PrecioCompra * detalle.Cantidad;
                        costoTotal += costoProducto;
                        utilidadTotal += detalle.Subtotal - costoProducto;
                    }
                }

                // Ventas por método de pago
                var ventasPorMetodoPago = ventas
                    .GroupBy(v => v.MetodoPago)
                    .Select(g => new VentasPorMetodoPagoDTO
                    {
                        MetodoPago = g.Key,
                        CantidadVentas = g.Count(),
                        TotalVentas = g.Sum(v => v.Total)
                    })
                    .OrderByDescending(v => v.TotalVentas)
                    .ToList();

                // Construir el reporte completo
                return new ReporteVentasDTO
                {
                    FechaGeneracion = DateTime.Now,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    TotalVentas = ventas.Count,
                    MontoTotalVentas = ventasTotal,
                    CostoTotal = costoTotal,
                    UtilidadTotal = utilidadTotal,
                    VentasPorDia = ventasPorDia,
                    ProductosMasVendidos = productosMasVendidos,
                    VentasPorMetodoPago = ventasPorMetodoPago
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al generar reporte de ventas: {ex.Message}");
                throw;
            }
        }
    }
}