using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.DataAccess.Abstract;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.DataAccess.Concrete.EntityFramework
{
    public class EfInvoiceDal : EfEntityRepositoryBase<Invoice , AyhanMakContext>, IInvoiceDal
    {
        public List<Invoice> getAllOrderById(Expression<Func<Invoice, bool>> filter = null)
        {
            using (AyhanMakContext context = new AyhanMakContext())
            {
                return filter == null ? context.Invoices.OrderByDescending(p => p.inovoiceid).ToList() : context.Invoices.OrderByDescending(p => p.inovoiceid).Where(filter).ToList();
            }
        }
    }
}
