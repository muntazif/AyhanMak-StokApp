using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Abstract;

namespace AyhanMak.Entities.Concrete
{
    public class Brand : IEntity
    {
        public int brandid { get; set; }
        public string brandname { get; set; }
    }
}
