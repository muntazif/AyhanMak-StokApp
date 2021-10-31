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
    public class Price : IEntity
    {
        public int pinovoiceid { get; set; }
        public int priceid { get; set; }
        public string colorcode { get; set; }
        public string productname { get; set; }
        public string productgenus { get; set; }
        public string productbrand { get; set; }
        public Decimal productprice { get; set; }
        public Decimal salesquantity { get; set; }
        public string unitid { get; set; }
        public bool maturity { get; set; }
    }
}
