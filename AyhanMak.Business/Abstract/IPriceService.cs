using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.Business.Abstract
{
    public interface IPriceService
    {
        List<Price> GetByInvoiceId(int faturaid);
        void Add(Price price);
    }
}
