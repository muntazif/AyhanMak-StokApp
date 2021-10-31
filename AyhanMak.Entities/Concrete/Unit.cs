using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Abstract;

namespace AyhanMak.Entities.Concrete
{
    public class Unit : IEntity
    {
        public int unitid { get; set; }
        public string unitname { get; set; }
    }
}
