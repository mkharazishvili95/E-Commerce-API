using E_Commerce_API.Data;
using E_Commerce_API.Models;
using E_Commerce_API.Validation;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace E_Commerce_API.Services
{
    public interface IAdminService
    {
        IEnumerable<User> GetAllUsers();
        User GetUserById(int userId);
        User SortUsersByAddress(string  address);
        User BlockUser (int userId);
        User UnblockUser(int userId);
        Product AddProduct(Product newProduct);
        bool UpdateProduct(Product updateProduct, int productId);
        bool DeleteProduct(int productId);
        bool CancelPurchase(int purchaseId);
    }
    public class AdminService : IAdminService
    {
        private readonly ECommerceContext _context;
        public AdminService(ECommerceContext context)
        {
            _context = context;
        }
        public User BlockUser(int userId)
        {
            var existingUser = _context.Users.SingleOrDefault(user => user.Id == userId);
            if(existingUser == null) {
                return null;
            }
            else
            {
                existingUser.IsBlocked = true;
                _context.Update(existingUser);
                _context.SaveChanges();
                return existingUser;
            }
        }
        public User UnblockUser(int userId)
        {
            var existingUser = _context.Users.SingleOrDefault(user => user.Id == userId);
            if(existingUser == null)
            {
                return null;
            }
            else
            {
                existingUser.IsBlocked = false;
                _context.Update(existingUser);
                _context.SaveChanges();
                return existingUser;
            }
        }

        public IEnumerable<User> GetAllUsers()
        {
            var userList = _context.Users;
            return userList;
        }

        public User SortUsersByAddress(string address)
        {
            var sortedUsersByAddress = _context.Users.FirstOrDefault(user => user.ShippingAddress.ToUpper().Contains(address.ToUpper()));
            return (sortedUsersByAddress);
        }
        public User GetUserById(int userId)
        {
            var existingUser = _context.Users.SingleOrDefault(user => user.Id == userId);
            if(existingUser == null)
            {
                return null;
            }
            else
            {
                return existingUser;
            }
        }

        public Product AddProduct(Product newProduct)
        {
            var productToAdd = new Product
            {
                Name = newProduct.Name,
                Quantity = newProduct.Quantity,
                Description = newProduct.Description,
                ProductCategory = newProduct.ProductCategory,
                InStock = newProduct.InStock,
                Price = newProduct.Price
            };
            _context.Add(productToAdd);
            _context.SaveChanges();
            return productToAdd;
        }

        public bool UpdateProduct(Product updateProduct, int productId)
        {
            var existingProduct = _context.Products.SingleOrDefault(product => product.Id == productId);
            existingProduct.Price = updateProduct.Price;
            existingProduct.Quantity = updateProduct.Quantity;
            existingProduct.ProductCategory = updateProduct.ProductCategory;
            existingProduct.Name = updateProduct.Name;
            existingProduct.Description = updateProduct.Description;
            _context.Update(existingProduct);
            _context.SaveChanges();
            return true;
        }
        public bool DeleteProduct(int productId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                var existingProduct = _context.Products.SingleOrDefault(product => product.Id == productId);

                if (existingProduct == null)
                {
                    return false;
                }
                var purchases = _context.Purchases.Where(x => x.ProductId == productId).ToList();
                foreach (var purchase in purchases)
                {
                    var user = _context.Users.FirstOrDefault(x => x.Id == purchase.UserId);
                    var receiverBank = _context.BankModels.FirstOrDefault() ?? new BankModel();

                    if (user != null && receiverBank != null)
                    {
                        var paymentCalculator = (existingProduct.Price * purchase.Quantity);
                        user.Balance += paymentCalculator;
                        receiverBank.BankStatement -= paymentCalculator;
                        _context.Update(user);
                        _context.Update(receiverBank);
                    }
                    _context.Remove(purchase);
                }
                _context.Remove(existingProduct);
                _context.SaveChanges();
                transaction.Commit();
                return true;
            }
        }
        public bool CancelPurchase(int purchaseId)
        {
            var purchase = _context.Purchases.FirstOrDefault(x => x.Id == purchaseId);
            if (purchase == null)
            {
                return false;
            }
            var product = _context.Products.FirstOrDefault(x => x.Id == purchase.ProductId);
            var user = _context.Users.FirstOrDefault(x => x.Id == purchase.UserId);
            var receiverBank = _context.BankModels.FirstOrDefault() ?? new BankModel();
            if (product == null || user == null || receiverBank == null)
            {
                return false;
            }
            var paymentCalculator = (product.Price * purchase.Quantity);
            user.Balance += paymentCalculator;
            receiverBank.BankStatement -= paymentCalculator;
            product.Quantity += purchase.Quantity;
            if (product.Quantity > 0 && product.InStock == "No")
            {
                product.InStock = "Yes";
            }
            _context.Update(user);
            _context.Update(receiverBank);
            _context.Update(product);
            _context.Remove(purchase);
            _context.SaveChanges();
            return true;
        }
    }
}
