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
    public class BrandManager : IBrandServices
    {
        private IBrandDal _brandDal = new EfBrandDal();

        public List<Brand> GetAll()
        {
            return _brandDal.GetAllOrderByBrandName();
        }

        public Brand Get(string brandName)
        {
            return _brandDal.Get(p => p.brandname == brandName);
        }

        public void Add(Brand brand)
        {
            _brandDal.Add(brand);
        }

        public void Update(Brand brand)
        {
            _brandDal.Update(brand);
        }

        public void Delete(Brand brand)
        {
            try
            {
                _brandDal.Delete(brand);
            }
            catch
            {
                throw new Exception("Silme gerçekleşemedi!");
            }
        }
    }
}
