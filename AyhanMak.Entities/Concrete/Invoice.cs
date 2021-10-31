using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Abstract;

namespace AyhanMak.Entities.Concrete
{
    [Table("Invoices")]
    public class Invoice : IEntity
    {
        [Key]
        public int inovoiceid { get; set; }
        public DateTime date { get; set; }
        public Decimal totalprice { get; set; }
        public string customer { get; set; }
        public string category { get; set; }
    }
}
