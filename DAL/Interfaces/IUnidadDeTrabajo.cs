using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUnidadDeTrabajo : IDisposable
    {
        IParametroDAL ParametroDAL { get; }

        IProductoDAL ProductoDAL { get; }

        IVentaDAL VentaDAL { get; }

        IDetalleVentaDAL DetalleVentaDAL { get; }

        IMovimientoInventarioDAL MovimientoInventarioDAL { get; }

        void Complete();

    }
}
