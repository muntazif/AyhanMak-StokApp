using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.DataAccess.Abstract
{
    public interface IProductDal : IEntityRepository<Product>
    {
        List<Product> GetAllOrderByBrand(Expression<Func<Product, bool>> filter = null);
        List<Product> getAllOrderByProductName();
    }
}
