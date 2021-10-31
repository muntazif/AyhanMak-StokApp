using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.DataAccess.Abstract;
using AyhanMak.Entities.Concrete;


namespace AyhanMak.DataAccess.Concrete.EntityFramework
{
    public class EfBrandDal : EfEntityRepositoryBase<Brand, AyhanMakContext>, IBrandDal
    {
        public List<Brand> GetAllOrderByBrandName()
        {
            using (AyhanMakContext context = new AyhanMakContext())
            {
                return context.Brands.OrderBy(p => p.brandname).ToList();
            }
        }
    }
}
