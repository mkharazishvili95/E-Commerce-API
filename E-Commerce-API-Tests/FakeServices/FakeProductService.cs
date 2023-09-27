using System;
using System.Collections.Generic;
using E_Commerce_API.Models;
using E_Commerce_API.Services;

namespace E_Commerce_API_Tests.FakeServices
{
    public class FakeProductService : IProductService
    {
        private readonly List<Product> _products;
        private readonly List<PurchaseModel> _purchases;

        public FakeProductService(List<Product> products, List<PurchaseModel> purchases)
        {
            _products = products;
            _purchases = purchases;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _products;
        }

        public IEnumerable<PurchaseModel> GetMyPurchases(int userId)
        {
            return _purchases.FindAll(p => p.UserId == userId);
        }

        public bool BuyProduct(int userId, PurchaseModel purchase)
        {
            var product = _products.Find(p => p.Id == purchase.ProductId);
            if (product == null)
            {
                return false;
            }

            var user = new User { Id = userId, Balance = 100 };
            var receiverBank = new BankModel();

            if (user.IsBlocked || product.Quantity == 0)
            {
                return false;
            }

            var paymentCalculator = (product.Price * purchase.Quantity);
            if (user.Balance < paymentCalculator || purchase.Quantity > product.Quantity)
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
            _purchases.Add(newPurchase);
            if (product.Quantity == 0)
            {
                product.InStock = "No";
            }
            return true;
        }
    }
}