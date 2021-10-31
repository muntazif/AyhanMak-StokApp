using AyhanMak.DataAccess.Abstract;
using AyhanMak.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AyhanMak.DataAccess.Concrete.EntityFramework
{
    public class EfLogDal : EfEntityRepositoryBase<Log, AyhanMakContext>, ILogDal
    {
    }
}
