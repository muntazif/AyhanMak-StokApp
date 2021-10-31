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
    public class InvoiceManager : IInvoiceService
    {
        private IInvoiceDal _invoiceDal = new EfInvoiceDal();

        public List<Invoice> GetAllById()
        {
            return _invoiceDal.getAllOrderById();
        }

        public List<Invoice> GetByDay(string dateTime)
        {
            return _invoiceDal.getAllOrderById(p => p.date.Day.ToString() == dateTime);
        }

        public List<Invoice> GetByMount(string dateTime)
        {
            return _invoiceDal.getAllOrderById(p => p.date.Month.ToString() == dateTime);
        }

        public List<Invoice> GetByCompany(string company)
        {
            return _invoiceDal.getAllOrderById(p => p.customer == company);
        }

        public List<Invoice> GetByCategory(string category)
        {
            return _invoiceDal.getAllOrderById(p => p.category == category);
        }

        public Invoice getByInvoiceId(int invoiceId)
        {
            return _invoiceDal.Get(p => p.inovoiceid == invoiceId);
        }

        public void Add(Invoice invoice)
        {
            _invoiceDal.Add(invoice);
        }
    }
}
