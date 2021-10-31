using AyhanMak.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyhanMak.Entities.Concrete
{
    public class Log : IEntity
    {
        [Key]
        public int logid { get; set; }
        public string logproductname { get; set; }
        public string logadding { get; set; }
        public string logselling { get; set; }
        public string logoldquantity { get; set; }
        public string lognewquantity { get; set; }
    }
}
