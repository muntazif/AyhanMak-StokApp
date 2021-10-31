using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.DataAccess.Abstract;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.DataAccess.Concrete.EntityFramework
{
    public class EfPriceDal : EfEntityRepositoryBase<Price, AyhanMakContext>, IPriceDal
    {
    }
}
