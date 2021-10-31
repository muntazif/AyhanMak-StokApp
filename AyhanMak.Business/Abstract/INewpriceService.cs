using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.Business.Abstract
{
    public interface INewpriceService
    {
        List<Newprice> GetByInvoiceId(int faturaid);
        List<Newprice> GetByInvoiceIdAndSellType(int faturaid, bool sellType);
        void Add(Newprice newprice);
    }
}
