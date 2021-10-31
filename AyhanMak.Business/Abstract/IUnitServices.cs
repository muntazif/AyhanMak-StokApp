using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.Business.Abstract
{
    public interface IUnitServices
    {
        List<Unit> GetAll();
        Unit Get(string unitName);
        void Add(Unit unit);
        void Update(Unit unit);
        void Delete(Unit unit);
    }
}
