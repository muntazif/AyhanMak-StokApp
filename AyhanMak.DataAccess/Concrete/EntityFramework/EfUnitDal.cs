using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.DataAccess.Abstract;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.DataAccess.Concrete.EntityFramework
{
    public class EfUnitDal : EfEntityRepositoryBase<Unit,AyhanMakContext>, IUnitDal
    {
        public List<Unit> GetAllOrderByUnitName()
        {
            using (AyhanMakContext context = new AyhanMakContext())
            {
                return context.Units.OrderBy(p => p.unitname).ToList();
            }
        }
    }
}
