using DAL.Interfaces;
using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implementations
{
    public class ParametroDALImpl : GenericDALImpl<Parametro>, IParametroDAL
    {
        SistemaInventarioVentasContext _context;

        public ParametroDALImpl(SistemaInventarioVentasContext context) : base(context)
        {
            _context = context;
        }



    }
}
