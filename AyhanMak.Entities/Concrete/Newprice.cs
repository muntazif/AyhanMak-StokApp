using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Abstract;

namespace AyhanMak.Entities.Concrete
{
    public class Newprice : IEntity
    {
        public int npinovoiceid { get; set; }
        [Key]
        public int npid { get; set; }
        public string npbrand { get; set; }
        public string npcomment { get; set; }
        public Decimal npquantity { get; set; }
        public Decimal npcash { get; set; }
        public Decimal nptotalprice { get; set; }
        public string npcompany { get; set; }
        public string npcontact { get; set; }
        public bool nptype { get; set; }
    }
}
