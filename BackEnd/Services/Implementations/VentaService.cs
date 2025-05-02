using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using DAL.Implementations;
using DAL.Interfaces;
using Entities.Entities;
using Microsoft.EntityFrameworkCore;

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

        // Método modificado para procesar la venta con el nuevo DTO de entrada
        public VentaDTO ProcesarVenta(CrearVentaDTO crearVentaDTO)
        {
            try
            {
                _logger.LogInformation("Iniciando procesamiento de venta");

                // Validar que la venta tenga detalles
                if (crearVentaDTO.DetalleVenta == null || !crearVentaDTO.DetalleVenta.Any())
                {
                    throw new Exception("La venta debe tener al menos un producto");
                }

                // Validar productos y calcular subtotales
                decimal subtotal = 0;
                List<(int ProductoId, int Cantidad, decimal PrecioUnitario, decimal Subtotal, decimal Descuento)> detallesCalculados = new List<(int, int, decimal, decimal, decimal)>();

                foreach (var detalle in crearVentaDTO.DetalleVenta)
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

                    // Calcular el subtotal del detalle
                    decimal precioUnitario = producto.PrecioVenta;
                    decimal subtotalDetalle = precioUnitario * detalle.Cantidad - detalle.Descuento;

                    detallesCalculados.Add((detalle.ProductoId, detalle.Cantidad, precioUnitario, subtotalDetalle, detalle.Descuento));
                    subtotal += subtotalDetalle;
                }

                // Calcular impuestos (13% por defecto)
                decimal impuestos = CalcularImpuestos(subtotal);
                decimal total = subtotal + impuestos;

                // Crear la venta
                var venta = new Venta
                {
                    FechaVenta = DateTime.Now,
                    Notas = crearVentaDTO.Notas,
                    NumeroFactura = GenerarNumeroFactura(),
                    Subtotal = subtotal,
                    Impuestos = impuestos,
                    Total = total,
                    MetodoPago = crearVentaDTO.MetodoPago,
                    EstadoVenta = "Completada"
                };

                // Guardar la venta
                _unidadDeTrabajo.VentaDAL.Add(venta);
                _unidadDeTrabajo.Complete(); // Necesitamos el ID de la venta para los detalles

                // Ahora procesamos los detalles
                foreach (var (productoId, cantidad, precioUnitario, subtotalDetalle, descuento) in detallesCalculados)
                {
                    var producto = _unidadDeTrabajo.ProductoDAL.FindById(productoId);

                    // Crear el detalle de venta
                    var detalle = new DetalleVenta
                    {
                        VentaId = venta.VentaId,
                        ProductoId = productoId,
                        Cantidad = cantidad,
                        PrecioUnitario = precioUnitario,
                        Descuento = descuento,
                        Subtotal = subtotalDetalle
                    };

                    // Guardar el detalle
                    _unidadDeTrabajo.DetalleVentaDAL.Add(detalle);

                    // Registrar el movimiento de inventario
                    // Asumimos que existe un tipo de movimiento para ventas con ID 2 (salida)
                    var movimiento = new MovimientoInventarioDTO
                    {
                        ProductoId = productoId,
                        Cantidad = cantidad,
                        FechaMovimiento = DateTime.Now,
                        TipoMovimientoId = 5, // Salida por venta
                        Notas = $"Venta #{venta.NumeroFactura}",
                        Referencia = venta.VentaId.ToString()
                    };

                    _movimientoService.AddMovimiento(movimiento);
                }

                // Guardar todos los cambios
                _unidadDeTrabajo.Complete();

                // Devolver la venta procesada convertida a DTO
                return ConvertirVentaCompletaADTO(venta);
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
                // Obtenemos las ventas junto con sus detalles en una sola consulta
                var context = _unidadDeTrabajo as UnidadDeTrabajo;
                var ventas = context.context.Venta
                    .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.Producto)
                    .OrderByDescending(v => v.FechaVenta)
                    .ToList();

                List<VentaDTO> resultado = new List<VentaDTO>();

                foreach (var venta in ventas)
                {
                    resultado.Add(ConvertirVentaCompletaADTO(venta));
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
                // Obtenemos la venta junto con sus detalles en una sola consulta
                var context = _unidadDeTrabajo as UnidadDeTrabajo;
                var venta = context.context.Venta
                    .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.Producto)
                    .FirstOrDefault(v => v.VentaId == id);

                if (venta == null)
                {
                    throw new Exception($"No existe la venta con ID {id}");
                }

                return ConvertirVentaCompletaADTO(venta);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener venta {id}: {ex.Message}");
                throw;
            }
        }

        public List<VentaDTO> GetVentasByFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                // Aseguramos que fechaFin sea el final del día
                fechaFin = fechaFin.Date.AddDays(1).AddSeconds(-1);

                // Obtenemos las ventas junto con sus detalles en una sola consulta
                var context = _unidadDeTrabajo as UnidadDeTrabajo;
                var ventas = context.context.Venta
                    .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.Producto)
                    .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin)
                    .OrderByDescending(v => v.FechaVenta)
                    .ToList();

                List<VentaDTO> resultado = new List<VentaDTO>();

                foreach (var venta in ventas)
                {
                    resultado.Add(ConvertirVentaCompletaADTO(venta));
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener ventas por fecha: {ex.Message}");
                throw;
            }
        }

        // Método auxiliar para convertir de Venta a VentaDTO incluyendo sus detalles
        private VentaDTO ConvertirVentaCompletaADTO(Venta venta)
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

            // Convertir los detalles de venta
            if (venta.DetalleVenta != null)
            {
                foreach (var detalle in venta.DetalleVenta)
                {
                    var detalleDTO = new DetalleVentaDTO
                    {
                        DetalleVentaId = detalle.DetalleVentaId,
                        VentaId = detalle.VentaId,
                        ProductoId = detalle.ProductoId,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        Subtotal = detalle.Subtotal,
                        Descuento = detalle.Descuento
                    };

                    ventaDTO.DetalleVenta.Add(detalleDTO);
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
    }
}