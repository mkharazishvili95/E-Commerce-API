using System.Collections.Generic;
using System.Linq;
using E_Commerce_API.Models;
using E_Commerce_API.Services;

namespace E_Commerce_API_Tests.FakeServices
{
    public class FakeAdminService : IAdminService
    {
        private readonly List<User> _users;
        private readonly List<Product> _products;
        private readonly List<PurchaseModel> _purchases;

        public FakeAdminService(List<User> users, List<Product> products, List<PurchaseModel> purchases)
        {
            _users = users;
            _products = products;
            _purchases = purchases;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _users;
        }

        public User GetUserById(int userId)
        {
            return _users.Find(u => u.Id == userId);
        }

        public User SortUsersByAddress(string address)
        {
            return _users.Find(u => u.ShippingAddress.ToUpper().Contains(address.ToUpper()));
        }

        public User BlockUser(int userId)
        {
            var user = _users.Find(u => u.Id == userId);
            if (user != null)
            {
                user.IsBlocked = true;
            }
            return user;
        }

        public User UnblockUser(int userId)
        {
            var user = _users.Find(u => u.Id == userId);
            if (user != null)
            {
                user.IsBlocked = false;
            }
            return user;
        }

        public Product AddProduct(Product newProduct)
        {
            _products.Add(newProduct);
            return newProduct;
        }

        public bool UpdateProduct(Product updateProduct, int productId)
        {
            var existingProduct = _products.Find(p => p.Id == productId);
            if (existingProduct != null)
            {
                existingProduct.Name = updateProduct.Name;
                existingProduct.Quantity = updateProduct.Quantity;
                return true;
            }
            return false;
        }

        public bool DeleteProduct(int productId)
        {
            var existingProduct = _products.Find(p => p.Id == productId);
            if (existingProduct != null)
            {
                _products.Remove(existingProduct);
                _purchases.RemoveAll(p => p.ProductId == productId);
                return true;
            }
            return false;
        }

        public bool CancelPurchase(int purchaseId)
        {
            var purchaseToRemove = _purchases.FirstOrDefault(p => p.Id == purchaseId);
            if (purchaseToRemove != null)
            {
                _purchases.Remove(purchaseToRemove);
                var product = _products.FirstOrDefault(p => p.Id == purchaseToRemove.ProductId);
                if (product != null)
                {
                    product.Quantity += purchaseToRemove.Quantity;
                }

                var user = _users.FirstOrDefault(u => u.Id == purchaseToRemove.UserId);
                if (user != null)
                {
                    var paymentCalculator = (purchaseToRemove.TotalPayment);
                    user.Balance += paymentCalculator;
                }

                return true;
            }

            return false;
        }
    }
}
