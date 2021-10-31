using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.Business.Abstract
{
    public interface IBrandServices
    {
        List<Brand> GetAll();
        Brand Get(string brandName);
        void Add(Brand brand);
        void Update(Brand brand);
        void Delete(Brand brand);
    }
}
