using AyhanMak.Business.Abstract;
using AyhanMak.DataAccess.Abstract;
using AyhanMak.DataAccess.Concrete.EntityFramework;
using AyhanMak.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyhanMak.Business.Concrete
{
    public class LogManager : ILogServices
    {
        ILogDal _efLogDal = new EfLogDal();
        public void Add(Log log)
        {
            _efLogDal.Add(log);
        }
    }
}
