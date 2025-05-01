using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using DAL.Interfaces;
using Entities.Entities;

namespace BackEnd.Services.Implementations
{
    public class VentaService : IVentaService
    {
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly ILogger<VentaService> _logger;
        private readonly IMovimientoInventarioService _movimientoService;

        public VentaService(
            IUnidadDeTrabajo unidadDeTrabajo,
            ILogger<VentaService> logger,
            IMovimientoInventarioService movimientoService)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _logger = logger;
            _movimientoService = movimientoService;
        }

        // Método para procesar una venta completa (cabecera y detalles)
        public VentaDTO ProcesarVenta(VentaDTO ventaDTO)
        {
            try
            {
                _logger.LogInformation("Iniciando procesamiento de venta");

                // Validar que la venta tenga detalles
                if (ventaDTO.DetalleVenta == null || !ventaDTO.DetalleVenta.Any())
                {
                    throw new Exception("La venta debe tener al menos un producto");
                }

                // Validar que los productos existan y tengan stock suficiente
                foreach (var detalle in ventaDTO.DetalleVenta)
                {
                    var producto = _unidadDeTrabajo.ProductoDAL.FindById(detalle.ProductoId);

                    if (producto == null)
                    {
                        throw new Exception($"El producto con ID {detalle.ProductoId} no existe");
                    }

                    if (producto.Stock < detalle.Cantidad)
                    {
                        throw new Exception($"Stock insuficiente para el producto '{producto.Nombre}'. Stock actual: {producto.Stock}, Cantidad solicitada: {detalle.Cantidad}");
                    }

                    // Calcular el subtotal si no viene calculado
                    if (detalle.Subtotal <= 0)
                    {
                        detalle.PrecioUnitario = producto.PrecioVenta;
                        detalle.Subtotal = detalle.PrecioUnitario * detalle.Cantidad - detalle.Descuento;
                    }
                }

                // Crear la venta
                var venta = new Venta
                {
                    FechaVenta = DateTime.Now,
                    Notas = ventaDTO.Notas,
                    NumeroFactura = GenerarNumeroFactura(),
                    Subtotal = ventaDTO.DetalleVenta.Sum(d => d.Subtotal),
                    Impuestos = CalcularImpuestos(ventaDTO.DetalleVenta.Sum(d => d.Subtotal)),
                    MetodoPago = ventaDTO.MetodoPago,
                    EstadoVenta = "Completada"
                };

                // Calcular total
                venta.Total = venta.Subtotal + venta.Impuestos;

                // Guardar la venta
                _unidadDeTrabajo.VentaDAL.Add(venta);
                _unidadDeTrabajo.Complete(); // Necesitamos el ID de la venta para los detalles

                // Ahora procesamos los detalles
                foreach (var detalleDTO in ventaDTO.DetalleVenta)
                {
                    var producto = _unidadDeTrabajo.ProductoDAL.FindById(detalleDTO.ProductoId);

                    // Crear el detalle de venta
                    var detalle = new DetalleVenta
                    {
                        VentaId = venta.VentaId,
                        ProductoId = detalleDTO.ProductoId,
                        Cantidad = detalleDTO.Cantidad,
                        PrecioUnitario = detalleDTO.PrecioUnitario,
                        Descuento = detalleDTO.Descuento,
                        Subtotal = detalleDTO.Subtotal
                    };

                    // Guardar el detalle
                    _unidadDeTrabajo.DetalleVentaDAL.Add(detalle);

                    // Actualizar el stock del producto
                    producto.Stock -= detalle.Cantidad;
                    producto.FechaModificacion = DateTime.Now;
                    _unidadDeTrabajo.ProductoDAL.Update(producto);

                    // Registrar el movimiento de inventario
                    // Asumimos que existe un tipo de movimiento para ventas con ID 2 (salida)
                    var movimiento = new MovimientoInventarioDTO
                    {
                        ProductoId = detalle.ProductoId,
                        Cantidad = detalle.Cantidad,
                        FechaMovimiento = DateTime.Now,
                        TipoMovimientoId = 2, // Salida por venta
                        Notas = $"Venta #{venta.NumeroFactura}",
                        Referencia = venta.VentaId.ToString()
                    };

                    _movimientoService.AddMovimiento(movimiento);
                }

                // Guardar todos los cambios
                _unidadDeTrabajo.Complete();

                // Devolver la venta procesada
                ventaDTO.VentaId = venta.VentaId;
                ventaDTO.FechaVenta = venta.FechaVenta;
                ventaDTO.NumeroFactura = venta.NumeroFactura;
                ventaDTO.Subtotal = venta.Subtotal;
                ventaDTO.Impuestos = venta.Impuestos;
                ventaDTO.Total = venta.Total;
                ventaDTO.EstadoVenta = venta.EstadoVenta;

                return ventaDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al procesar la venta: {ex.Message}");
                throw;
            }
        }

        public List<VentaDTO> GetVentas()
        {
            try
            {
                var ventas = _unidadDeTrabajo.VentaDAL.Get()
                    .OrderByDescending(v => v.FechaVenta)
                    .ToList();

                List<VentaDTO> resultado = new List<VentaDTO>();

                foreach (var venta in ventas)
                {
                    resultado.Add(ConvertirVenta(venta));
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener ventas: {ex.Message}");
                throw;
            }
        }

        public VentaDTO GetVentaById(int id)
        {
            try
            {
                var venta = _unidadDeTrabajo.VentaDAL.FindById(id);

                if (venta == null)
                {
                    throw new Exception($"No existe la venta con ID {id}");
                }

                return ConvertirVenta(venta);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener venta {id}: {ex.Message}");
                throw;
            }
        }

        // Método auxiliar para convertir de Venta a VentaDTO
        private VentaDTO ConvertirVenta(Venta venta)
        {
            var ventaDTO = new VentaDTO
            {
                VentaId = venta.VentaId,
                FechaVenta = venta.FechaVenta,
                Notas = venta.Notas,
                NumeroFactura = venta.NumeroFactura,
                Subtotal = venta.Subtotal,
                Impuestos = venta.Impuestos,
                Total = venta.Total,
                MetodoPago = venta.MetodoPago,
                EstadoVenta = venta.EstadoVenta,
                DetalleVenta = new List<DetalleVentaDTO>()
            };

            // Obtener los detalles de la venta (esto asume que Entity Framework carga la relación)
            if (venta.DetalleVenta != null)
            {
                foreach (var detalle in venta.DetalleVenta)
                {
                    ventaDTO.DetalleVenta.Add(new DetalleVentaDTO
                    {
                        DetalleVentaId = detalle.DetalleVentaId,
                        VentaId = detalle.VentaId,
                        ProductoId = detalle.ProductoId,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        Subtotal = detalle.Subtotal,
                        Descuento = detalle.Descuento
                    });
                }
            }

            return ventaDTO;
        }

        // Método para generar un número de factura único
        private string GenerarNumeroFactura()
        {
            // Formato: INV-AÑO-MES-SECUENCIAL
            var fecha = DateTime.Now;
            var secuencial = _unidadDeTrabajo.VentaDAL.Get().Count() + 1;

            return $"INV-{fecha.Year}-{fecha.Month:D2}-{secuencial:D4}";
        }

        // Método para calcular impuestos (ejemplo con 13% de IVA)
        private decimal CalcularImpuestos(decimal subtotal)
        {
            return Math.Round(subtotal * 0.13m, 2);
        }

        public List<VentaDTO> GetVentasByFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var ventas = _unidadDeTrabajo.VentaDAL.Get()
                    .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin)
                    .OrderByDescending(v => v.FechaVenta)
                    .ToList();

                List<VentaDTO> resultado = new List<VentaDTO>();

                foreach (var venta in ventas)
                {
                    resultado.Add(ConvertirVenta(venta));
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener ventas por fecha: {ex.Message}");
                throw;
            }
        }
    }
}
