using DAL.Interfaces;
using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implementations
{
    public class MovimientoInventarioDALImpl : GenericDALImpl<MovimientoInventario>, IMovimientoInventarioDAL
    {
        SistemaInventarioVentasContext _context;

        public MovimientoInventarioDALImpl(SistemaInventarioVentasContext context) : base(context)
        {
            _context = context;
        }



    }
}
