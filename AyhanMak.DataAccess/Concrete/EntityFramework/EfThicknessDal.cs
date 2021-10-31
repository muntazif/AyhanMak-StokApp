using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.DataAccess.Abstract;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.DataAccess.Concrete.EntityFramework
{
    public class EfThicknessDal : EfEntityRepositoryBase<Thickness, AyhanMakContext>, IThicknessDal
    {
        public List<Thickness> getAllOrderByThicknessName()
        {
            using (AyhanMakContext context = new AyhanMakContext())
            {
                return context.Thicknesses.OrderBy(p => p.thicknessname).ToList();
            }
        }

    }
}
