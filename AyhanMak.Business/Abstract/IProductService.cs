using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.Business.Abstract
{
    public interface IProductService
    {
        List<Product> GetProductByBrand();
        List<Product> GetProductByBrandName(string markaName);
        List<Product> getAllByProductName();
        List<Product> GetProductByBrandNameAndThickness(string markaName, string kalinlikName);
        List<Product> GetProductByProductName(string productName);
        List<Product> GetProductByAllProductId(string productId);
        List<Product> GetProductByAllProductThickness(string thickness);
        Product GetProductByColorCodeAndBrandAndThickness(string renkKodu, string marka, string kalinlik, string productName);
        void Add(Product product);
        void Update(Product product);
        void Delete(Product product);
    }
}
