using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.DataAccess.Abstract
{
    public interface IInvoiceDal : IEntityRepository<Invoice>
    {
        List<Invoice> getAllOrderById(Expression<Func<Invoice, bool>> filter = null);
    }
}
