using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.Business.Abstract
{
    public interface IThicknessServices
    {
        List<Thickness> GetAll();
        Thickness Get(string thicknessName);
        void Add(Thickness thickness);
        void Update(Thickness thickness);
        void Delete(Thickness thickness);
    }
}
