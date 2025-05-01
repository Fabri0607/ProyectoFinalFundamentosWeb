using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using DAL.Interfaces;
using Entities.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BackEnd.Services.Implementations
{
    public class VentaService : IVentaService
    {
        ILogger<VentaService> _logger;
        IUnidadDeTrabajo _unidadDeTrabajo;

        public VentaService(IUnidadDeTrabajo unidad,
                            ILogger<VentaService> logger)
        {
            _unidadDeTrabajo = unidad;
            _logger = logger;
        }

        VentaDTO Convertir(Venta venta)
        {
            return new VentaDTO
            {
                VentaId = venta.VentaId,
                FechaVenta = venta.FechaVenta,
                Notas = venta.Notas,
                NumeroFactura = venta.NumeroFactura,
                Subtotal = venta.Subtotal,
                Impuestos = venta.Impuestos,
                Total = venta.Total,
                MetodoPago = venta.MetodoPago,
                EstadoVenta = venta.EstadoVenta
            };
        }

        Venta Convertir(VentaDTO ventaDTO)
        {
            return new Venta
            {
                VentaId = ventaDTO.VentaId,
                FechaVenta = ventaDTO.FechaVenta,
                Notas = ventaDTO.Notas,
                NumeroFactura = ventaDTO.NumeroFactura,
                Subtotal = ventaDTO.Subtotal,
                Impuestos = ventaDTO.Impuestos,
                Total = ventaDTO.Total,
                MetodoPago = ventaDTO.MetodoPago,
                EstadoVenta = ventaDTO.EstadoVenta
            };
        }

        public VentaDTO AddVenta(VentaDTO ventaDTO)
        {
            try
            {
                _logger.LogError("Ingresa a AddVenta");
                _unidadDeTrabajo.VentaDAL.Add(Convertir(ventaDTO));
                _unidadDeTrabajo.Complete();
                return ventaDTO;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public VentaDTO DeleteVenta(int id)
        {
            var venta = new Venta { VentaId = id };
            _unidadDeTrabajo.VentaDAL.Remove(venta);
            _unidadDeTrabajo.Complete();
            return Convertir(venta);
        }

        public List<VentaDTO> GetVentas()
        {
            var ventas = _unidadDeTrabajo.VentaDAL.Get();
            List<VentaDTO> ventaDTOs = new List<VentaDTO>();
            foreach (var venta in ventas)
            {
                ventaDTOs.Add(Convertir(venta));
            }
            return ventaDTOs;
        }

        public VentaDTO GetVentaById(int id)
        {
            var result = _unidadDeTrabajo.VentaDAL.FindById(id);
            return Convertir(result);
        }

        public VentaDTO UpdateVenta(VentaDTO ventaDTO)
        {
            var entity = Convertir(ventaDTO);
            _unidadDeTrabajo.VentaDAL.Update(entity);
            _unidadDeTrabajo.Complete();
            return ventaDTO;
        }
    }
}

