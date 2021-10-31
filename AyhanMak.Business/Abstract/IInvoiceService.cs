using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Concrete;


namespace AyhanMak.Business.Abstract
{
    public interface IInvoiceService
    {
        List<Invoice> GetAllById();
        List<Invoice> GetByDay(string dateTime);
        List<Invoice> GetByMount(string dateTime);
        List<Invoice> GetByCompany(string company);
        List<Invoice> GetByCategory(string category);
        Invoice getByInvoiceId(int invoiceId);
        void Add(Invoice invoice);
    }
}
