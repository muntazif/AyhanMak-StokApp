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
    public class NewpriceManager : INewpriceService
    {
        private INewpriceDal _newpriceDal = new EfNewpriceDal();
        public List<Newprice> GetByInvoiceId(int faturaid)
        {
            return _newpriceDal.GetAll(p => p.npinovoiceid == faturaid);
        }

        public void Add(Newprice newprice)
        {
            _newpriceDal.Add(newprice);
        }
        public List<Newprice> GetByInvoiceIdAndSellType(int faturaid, bool sellType)
        {
            return _newpriceDal.GetAll(p => p.npinovoiceid == faturaid && p.nptype == sellType);
        }

    }
}
