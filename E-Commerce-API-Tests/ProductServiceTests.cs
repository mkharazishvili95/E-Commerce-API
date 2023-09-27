using System.Collections.Generic;
using System.Linq;
using E_Commerce_API.Models;
using E_Commerce_API.Services;
using E_Commerce_API_Tests.FakeServices;
using NUnit.Framework;

namespace E_Commerce_API_Tests
{
    [TestFixture]
    public class ProductServiceTests
    {
        [Test]
        public void GetAllProducts_ReturnsAllProducts()
        {
            List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 10.0, Quantity = 5, InStock = "Yes" },
                new Product { Id = 2, Name = "Product 2", Price = 20.0, Quantity = 3, InStock = "Yes" }
            };
            IProductService productService = new FakeProductService(products, new List<PurchaseModel>());
            IEnumerable<Product> result = productService.GetAllProducts();
            Assert.AreEqual(2, result?.Count());
        }
        [Test]
        public void GetMyPurchases_ReturnsPurchasesForUser()
        {
            List<PurchaseModel> purchases = new List<PurchaseModel>
            {
                new PurchaseModel { Id = 1, UserId = 1, ProductId = 1, Quantity = 2 },
                new PurchaseModel { Id = 2, UserId = 2, ProductId = 2, Quantity = 3 },
                new PurchaseModel { Id = 3, UserId = 1, ProductId = 2, Quantity = 1 }
            };
            IProductService productService = new FakeProductService(new List<Product>(), purchases);
            IEnumerable<PurchaseModel> user1Purchases = productService.GetMyPurchases(1);
            IEnumerable<PurchaseModel> user2Purchases = productService.GetMyPurchases(2);
            Assert.AreEqual(2, user1Purchases?.Count());
            Assert.AreEqual(1, user2Purchases?.Count());
        }

        [Test]
        public void BuyProduct_ReturnsTrueIfPurchaseIsSuccessful()
        {
            List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 10.0, Quantity = 5, InStock = "Yes" }
            };
            List<PurchaseModel> purchases = new List<PurchaseModel>();
            IProductService productService = new FakeProductService(products, purchases);
            PurchaseModel purchase = new PurchaseModel { UserId = 1, ProductId = 1, Quantity = 2 };
            bool purchaseResult = productService.BuyProduct(1, purchase);
            Assert.IsTrue(purchaseResult);
        }

        [Test]
        public void BuyProduct_ReturnsFalseIfProductNotFound()
        {
            List<Product> products = new List<Product>();
            List<PurchaseModel> purchases = new List<PurchaseModel>();
            IProductService productService = new FakeProductService(products, purchases);
            PurchaseModel purchase = new PurchaseModel { UserId = 1, ProductId = 1, Quantity = 2 };
            bool purchaseResult = productService.BuyProduct(1, purchase);
            Assert.IsFalse(purchaseResult);
        }

        [Test]
        public void BuyProduct_ReturnsFalseIfInsufficientBalance()
        {
            List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 10.0, Quantity = 5, InStock = "Yes" }
            };
            List<PurchaseModel> purchases = new List<PurchaseModel>();
            IProductService productService = new FakeProductService(products, purchases);
            PurchaseModel purchase = new PurchaseModel { UserId = 1, ProductId = 1, Quantity = 10 };
            bool purchaseResult = productService.BuyProduct(1, purchase);
            Assert.IsFalse(purchaseResult);
        }


    }
}
