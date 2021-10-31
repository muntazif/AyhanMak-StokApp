using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.DataAccess.Abstract
{
    public interface IThicknessDal : IEntityRepository<Thickness>
    {
        List<Thickness> getAllOrderByThicknessName();

    }
}
