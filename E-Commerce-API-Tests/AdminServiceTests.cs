using System.Collections.Generic;
using System.Linq;
using E_Commerce_API.Models;
using E_Commerce_API.Services;
using E_Commerce_API_Tests.FakeServices;
using NUnit.Framework;

namespace E_Commerce_API_Tests
{
    [TestFixture]
    public class AdminServiceTests
    {
        [Test]
        public void BlockUser_UserExists_ShouldBlockUser()
        {
            List<User> users = new List<User>
            {
                new User { Id = 1, UserName = "user1", IsBlocked = false },
                new User { Id = 2, UserName = "user2", IsBlocked = false }
            };
            IAdminService adminService = new FakeAdminService(users, new List<Product>(), new List<PurchaseModel>());
            User blockedUser = adminService.BlockUser(1);
            Assert.IsTrue(blockedUser.IsBlocked);
        }

        [Test]
        public void UnblockUser_UserExists_ShouldUnblockUser()
        {
            List<User> users = new List<User>
            {
                new User { Id = 1, UserName = "user1", IsBlocked = true },
                new User { Id = 2, UserName = "user2", IsBlocked = false }
            };
            IAdminService adminService = new FakeAdminService(users, new List<Product>(), new List<PurchaseModel>());
            User unblockedUser = adminService.UnblockUser(1);
            Assert.IsFalse(unblockedUser.IsBlocked);
        }

        [Test]
        public void AddProduct_ValidProduct_ShouldReturnNewProduct()
        {
            List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Quantity = 10, Price = 20.0, InStock = "Yes" }
            };
            IAdminService adminService = new FakeAdminService(new List<User>(), products, new List<PurchaseModel>());
            Product newProduct = new Product { Name = "Product 2", Quantity = 5, Price = 30.0, InStock = "Yes" };
            Product addedProduct = adminService.AddProduct(newProduct);
            Assert.AreEqual("Product 2", addedProduct.Name);
            Assert.AreEqual(5, addedProduct.Quantity);
            Assert.AreEqual(30.0, addedProduct.Price);
            Assert.AreEqual("Yes", addedProduct.InStock);
        }

        [Test]
        public void UpdateProduct_ProductExists_ShouldReturnTrue()
        {
            List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Quantity = 10, Price = 25.00, InStock = "Yes" }
            };
            IAdminService adminService = new FakeAdminService(new List<User>(), products, new List<PurchaseModel>());
            Product updatedProduct = new Product { Name = "Updated Product", Quantity = 15, Price = 25.00, InStock = "Yes" };
            bool result = adminService.UpdateProduct(updatedProduct, 1);
            Assert.IsTrue(result);
            Assert.AreEqual("Updated Product", products[0].Name);
            Assert.AreEqual(15, products[0].Quantity);
            Assert.AreEqual(25.00, products[0].Price);
            Assert.AreEqual("Yes", products[0].InStock);
        }

        [Test]
        public void DeleteProduct_ProductExists_ShouldReturnTrue()
        {
            List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Quantity = 10, Price = 20.0, InStock = "Yes" }
            };

            List<PurchaseModel> purchases = new List<PurchaseModel>
            {
                new PurchaseModel { Id = 1, UserId = 1, ProductId = 1, Quantity = 2 }
            };
            IAdminService adminService = new FakeAdminService(new List<User>(), products, purchases);
            bool result = adminService.DeleteProduct(1);
            Assert.IsTrue(result);
            Assert.AreEqual(0, products.Count);
            Assert.AreEqual(0, purchases.Count);
        }
        [Test]
        public void GetAllUsers_ShouldReturnAllUsers()
        {
            List<User> users = new List<User>
    {
        new User { Id = 1, UserName = "user1", IsBlocked = false, ShippingAddress = "Address 1" },
        new User { Id = 2, UserName = "user2", IsBlocked = false, ShippingAddress = "Address 2" }
    };
            IAdminService adminService = new FakeAdminService(users, new List<Product>(), new List<PurchaseModel>());
            IEnumerable<User> allUsers = adminService.GetAllUsers();
            Assert.AreEqual(2, allUsers.Count());
        }

        [Test]
        public void GetUserById_UserExists_ShouldReturnUser()
        {
            List<User> users = new List<User>
    {
        new User { Id = 1, UserName = "user1", IsBlocked = false, ShippingAddress = "Address 1" },
        new User { Id = 2, UserName = "user2", IsBlocked = false, ShippingAddress = "Address 2" }
    };
            IAdminService adminService = new FakeAdminService(users, new List<Product>(), new List<PurchaseModel>());
            User user = adminService.GetUserById(1);
            Assert.IsNotNull(user);
            Assert.AreEqual("user1", user.UserName);
        }

        [Test]
        public void SortUsersByAddress_AddressExists_ShouldReturnSortedUser()
        {
            List<User> users = new List<User>
    {
        new User { Id = 1, UserName = "user1", IsBlocked = false, ShippingAddress = "Address 1" },
        new User { Id = 2, UserName = "user2", IsBlocked = false, ShippingAddress = "Address 2" }
    };
            IAdminService adminService = new FakeAdminService(users, new List<Product>(), new List<PurchaseModel>());
            User sortedUser = adminService.SortUsersByAddress("Address 1");
            Assert.IsNotNull(sortedUser);
            Assert.AreEqual("user1", sortedUser.UserName);
        }
        [Test]
        public void CancelPurchase_PurchaseExists_ShouldCancelPurchase()
        {
            List<PurchaseModel> purchases = new List<PurchaseModel>
    {
        new PurchaseModel { Id = 1, UserId = 1, ProductId = 1, Quantity = 2, TotalPayment = 40.0 }
    };

            List<Product> products = new List<Product>
    {
        new Product { Id = 1, Name = "Product 1", Quantity = 10, Price = 20.0, InStock = "Yes" }
    };

            List<User> users = new List<User>
    {
        new User { Id = 1, UserName = "user1", IsBlocked = false, ShippingAddress = "Address 1", Balance = 100.0 }
    };

            IAdminService adminService = new FakeAdminService(users, products, purchases);
            bool result = adminService.CancelPurchase(1);
            Assert.IsTrue(result);
            PurchaseModel cancelledPurchase = purchases.FirstOrDefault(p => p.Id == 1);
            Assert.IsNull(cancelledPurchase);
            User user = users.FirstOrDefault(u => u.Id == 1);
            Assert.IsNotNull(user);
            Assert.AreEqual(140.0, user.Balance);
            Product product = products.FirstOrDefault(p => p.Id == 1);
            Assert.IsNotNull(product);
            Assert.AreEqual(12, product.Quantity);
        }
    }
}
