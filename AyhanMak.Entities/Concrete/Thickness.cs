using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Abstract;

namespace AyhanMak.Entities.Concrete
{
    public class Thickness : IEntity
    {
        public int thicknessid { get; set; }
        public string thicknessname { get; set; }
    }
}
