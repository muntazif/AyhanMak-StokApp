using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.DataAccess.Concrete.EntityFramework
{
    public class AyhanMakContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(p => p.stockquantity).HasPrecision(13, 4);
            modelBuilder.Entity<Product>().Property(p => p.maturityprice).HasPrecision(13, 4);
            modelBuilder.Entity<Product>().Property(p => p.cashprice).HasPrecision(13, 4);
            modelBuilder.Entity<Invoice>().Property(p => p.totalprice).HasPrecision(13, 4);
            modelBuilder.Entity<Price>().Property(p => p.productprice).HasPrecision(13, 4);
            modelBuilder.Entity<Price>().Property(p => p.salesquantity).HasPrecision(13, 4);
            modelBuilder.Entity<Newprice>().Property(p => p.npcash).HasPrecision(13, 4);
            modelBuilder.Entity<Newprice>().Property(p => p.npquantity).HasPrecision(13, 4);
            modelBuilder.Entity<Newprice>().Property(p => p.nptotalprice).HasPrecision(13, 4);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Newprice> Newprices { get; set; }
        public DbSet<Brand> Brands { get; set; }    
        public DbSet<Thickness> Thicknesses { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}
