using DAL.Interfaces;
using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implementations
{
    public class VentaDALImpl : GenericDALImpl<Venta>, IVentaDAL
    {
        SistemaInventarioVentasContext _context;

        public VentaDALImpl(SistemaInventarioVentasContext context) : base(context)
        {
            _context = context;
        }



    }
}
