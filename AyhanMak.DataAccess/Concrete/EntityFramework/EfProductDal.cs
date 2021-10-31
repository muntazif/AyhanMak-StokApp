using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.DataAccess.Abstract;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.DataAccess.Concrete.EntityFramework
{
    public class EfProductDal : EfEntityRepositoryBase<Product, AyhanMakContext>, IProductDal
    {
        public List<Product> GetAllOrderByBrand(Expression<Func<Product, bool>> filter = null)
        {
            using (AyhanMakContext context = new AyhanMakContext())
            {
                return filter == null
                    ? context.Products.OrderBy(p => p.brandname4pro).ToList()
                    : context.Products.OrderBy(p => p.brandname4pro).Where(filter).ToList();
            }
        }
        public List<Product> getAllOrderByProductName()
        {
            using (AyhanMakContext context = new AyhanMakContext())
            {
                return context.Products.OrderBy(p => p.productname).ToList();
            }
        }
    }
}
