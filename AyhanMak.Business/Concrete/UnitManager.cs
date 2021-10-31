using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Business.Abstract;
using AyhanMak.DataAccess.Abstract;
using AyhanMak.DataAccess.Concrete.EntityFramework;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.Business.Concrete
{
    public class UnitManager: IUnitServices
    {
        private IUnitDal _unitDal = new EfUnitDal();

        public List<Unit> GetAll()
        {
            return _unitDal.GetAllOrderByUnitName();
        }

        public Unit Get(string unitName)
        {
            return _unitDal.Get(p => p.unitname == unitName);
        }

        public void Add(Unit unit)
        {
            _unitDal.Add(unit);
        }

        public void Update(Unit unit)
        {
            _unitDal.Update(unit);
        }

        public void Delete(Unit unit)
        {
            try
            {
                _unitDal.Delete(unit);
            }
            catch
            {
                throw new Exception("Silme gerçekleşemedi!");
            }
        }
    }
}
