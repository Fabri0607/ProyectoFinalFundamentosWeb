using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IGenericDAL<T> where T : class
    {
        List<T> Get();
        bool Add(T entity);
        bool Remove(T entity);
        bool Update(T entity);
        T FindById(int id);


    }


}
