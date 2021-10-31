using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Abstract;

namespace AyhanMak.Entities.Concrete
{
    public class Product : IEntity
    {
        public int id { get; set; }
        public string colorcode { get; set; }
        public string brandname4pro { get; set; }
        public string thicknessname4pro { get; set; }
        public string productname { get; set; }
        public Decimal maturityprice { get; set; }
        public Decimal cashprice { get; set; }
        public Decimal stockquantity { get; set; }
        public string unitname4pro { get; set; }
        public int brandid4pro { get; set; }
        public int thicknessid4pro { get; set; }
        public int unitid4pro { get; set; }
    }
}
