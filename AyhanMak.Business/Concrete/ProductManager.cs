using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyhanMak.Business.Abstract;
using AyhanMak.DataAccess.Abstract;
using AyhanMak.DataAccess.Concrete.EntityFramework;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.Business.Concrete
{
    public class ProductManager : IProductService
    {
        private IProductDal _productDal = new EfProductDal();

        public List<Product> GetProductByBrand()
        {
            return _productDal.GetAllOrderByBrand();
        }

        public List<Product> GetProductByBrandName(string markaName)
        {
            return _productDal.GetAllOrderByBrand(p => p.brandname4pro == markaName);
        }

        public List<Product> getAllByProductName()
        {
            return _productDal.getAllOrderByProductName();
        }

        public List<Product> GetProductByBrandNameAndThickness(string markaName, string kalinlikName)
        {
            return _productDal.GetAllOrderByBrand(p => p.brandname4pro == markaName && p.thicknessname4pro == kalinlikName);
        }

        public List<Product> GetProductByProductName(string productName)
        {
            return _productDal.GetAll(p => p.productname == productName);
        }

        public List<Product> GetProductByAllProductId(string productId)
        {
            return _productDal.GetAll(p => p.colorcode == productId);
        }

        public List<Product> GetProductByAllProductThickness(string thickness)
        {
            return _productDal.GetAll(p => p.thicknessname4pro == thickness);
        }

        public Product GetProductByColorCodeAndBrandAndThickness(string renkKodu, string marka, string kalinlik, string productName)
        {
            return _productDal.Get(p =>p.colorcode == renkKodu && p.brandname4pro == marka && p.thicknessname4pro == kalinlik && p.productname == productName);
        }
        public void Add(Product product)
        {
            _productDal.Add(product);
        }

        public void Update(Product product)
        {
            _productDal.Update(product);
        }

        public void Delete(Product product)
        {
            try
            {
                _productDal.Delete(product);
            }
            catch
            {
                throw new Exception("Silme gerçekleşemedi!");
            }
        }
    }
}
