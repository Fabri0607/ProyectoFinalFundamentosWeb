using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using Entities.Entities;

namespace DAL.Implementations
{
    public class UnidadDeTrabajo : IUnidadDeTrabajo
    {
        public IParametroDAL ParametroDAL { get; set; }

        public IProductoDAL ProductoDAL { get; set; }

        public IVentaDAL VentaDAL { get; set; }

        public IDetalleVentaDAL DetalleVentaDAL { get; set; }

        public IMovimientoInventarioDAL MovimientoInventarioDAL { get; set; }


        public SistemaInventarioVentasContext context;

        public UnidadDeTrabajo(IParametroDAL parametroDAL, SistemaInventarioVentasContext context, 
            IProductoDAL productoDAL, IVentaDAL ventaDAL, IDetalleVentaDAL detalleVentaDAL, 
            IMovimientoInventarioDAL movimientoInventarioDAL)
        {
            ParametroDAL = parametroDAL;
            this.context = context;
            ProductoDAL = productoDAL;
            VentaDAL = ventaDAL;
            DetalleVentaDAL = detalleVentaDAL;
            MovimientoInventarioDAL = movimientoInventarioDAL;
        }
        public void Dispose()
        {
            this.context.Dispose();
        }

        public void Complete()
        {
            context.SaveChanges();
        }
    }
}
