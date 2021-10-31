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
    public class ThicknessManager : IThicknessServices
    {
        private IThicknessDal _thicknessDal = new EfThicknessDal();

        public List<Thickness> GetAll()
        {
            return _thicknessDal.getAllOrderByThicknessName();
        }

        public Thickness Get(string thicknessName)
        {
            return _thicknessDal.Get(p => p.thicknessname == thicknessName);
        }

        public void Add(Thickness thickness)
        {
            _thicknessDal.Add(thickness);
        }

        public void Update(Thickness thickness)
        {
            _thicknessDal.Update(thickness);
        }

        public void Delete(Thickness thickness)
        {
            try
            {
                _thicknessDal.Delete(thickness);
            }
            catch
            {
                throw new Exception("Silme gerçekleşemedi! Bir ürün tarafından kullanılıyor olabilir");
            }
        }
    }
}
