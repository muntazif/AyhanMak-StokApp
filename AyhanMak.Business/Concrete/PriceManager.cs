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
    public class PriceManager : IPriceService
    {
        private IPriceDal _priceDal = new EfPriceDal();
        public List<Price> GetByInvoiceId(int faturaid)
        {
            return _priceDal.GetAll(p => p.pinovoiceid == faturaid);
        }

        public void Add(Price price)
        {
            _priceDal.Add(price);
        }
    }
}
