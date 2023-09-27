using E_Commerce_API.Data;
using E_Commerce_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace E_Commerce_API.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<PurchaseModel> GetMyPurchases(int userId);
        bool BuyProduct(int userId, PurchaseModel purchase);
    }
    public class ProductService : IProductService
    {
        private readonly ECommerceContext _context;
        public ProductService(ECommerceContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            var productList = _context.Products.ToList();
            if(productList == null)
            {
                return null;
            }
            else
            {
                return productList;
            }
        }

        public IEnumerable<PurchaseModel> GetMyPurchases(int userId)
        {
            var myPurchases = _context.Purchases.Where(x => x.UserId == userId).ToList();
            return myPurchases;
        }
        public bool BuyProduct(int userId, PurchaseModel purchase)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            var product = _context.Products.FirstOrDefault(x => x.Id == purchase.ProductId);
            var receiverBank = _context.BankModels.FirstOrDefault() ?? new BankModel();

            if (product == null)
            {
                return false;
            }
            if (user == null)
            {
                return false;
            }
            if (receiverBank == null)
            {
                return false;
            }
            if (user.IsBlocked)
            {
                return false;
            }
            if (product.Quantity == 0)
            {
                return false;
            }
            var paymentCalculator = (product.Price * purchase.Quantity);
            if (user.Balance < paymentCalculator)
            {
                return false;
            }
            if (purchase.Quantity > product.Quantity)
            {
                return false;
            }
            user.Balance -= paymentCalculator;
            receiverBank.BankStatement += paymentCalculator;
            product.Quantity -= purchase.Quantity;
            var newPurchase = new PurchaseModel
            {
                UserId = userId,
                BuyingDate = DateTime.UtcNow,
                ProductId = product.Id,
                TotalPayment = paymentCalculator,
                Quantity = purchase.Quantity
            };
            _context.Update(user);
            _context.Update(receiverBank);
            _context.Update(product);
            _context.Add(newPurchase);
            _context.SaveChanges();

            if (product.Quantity == 0)
            {
                product.InStock = "No";
                _context.Update(product);
                _context.SaveChanges();
            }
            return true;
        }
    }
}
