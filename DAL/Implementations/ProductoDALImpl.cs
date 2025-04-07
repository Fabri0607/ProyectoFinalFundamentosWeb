using DAL.Interfaces;
using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implementations
{
    public class ProductoDALImpl : GenericDALImpl<Producto>, IProductoDAL
    {
        SistemaInventarioVentasContext _context;

        public ProductoDALImpl(SistemaInventarioVentasContext context) : base(context)
        {
            _context = context;
        }



    }
}
