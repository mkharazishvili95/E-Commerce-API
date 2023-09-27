using E_Commerce_API.Data;
using E_Commerce_API.Models;
using E_Commerce_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ECommerceContext _context;
        private readonly IProductService _productService;
        public ProductController(ECommerceContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }
        [Authorize]
        [HttpGet("GetAllProducts")]
        public IEnumerable<Product> GetAllProducts()
        {
            var productList = _productService.GetAllProducts();
            return productList;
        }
        [Authorize]
        [HttpPost("BuyProduct")]
        public IActionResult BuyProduct(PurchaseModel purchase)
        {
            var userId = int.Parse(User.Identity.Name);
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            var product = _context.Products.FirstOrDefault(x => x.Id == purchase.ProductId);
            var receiverBank = _context.BankModels.FirstOrDefault() ?? new BankModel();

            if (product == null)
            {
                return BadRequest(new { Message = "There is no any product by this ID!" });
            }

            if (user == null)
            {
                return BadRequest(new { Message = "User not found!" });
            }

            if (receiverBank == null)
            {
                return BadRequest(new { Message = "Receiver bank not found!" });
            }

            if (user.IsBlocked)
            {
                return BadRequest(new { Message = "You cannot buy any product because you are blocked!" });
            }
            if(product.Quantity == 0)
            {
                return BadRequest(new { Message = "This product is out of stock!" });
            }

            var paymentCalculator = (product.Price * purchase.Quantity);

            if (user.Balance < paymentCalculator)
            {
                return BadRequest(new { Message = $"You do not have enough money to buy this product. It costs: {paymentCalculator} Gel and your balance is: {user.Balance} Gel" });
            }
            if(purchase.Quantity > product.Quantity)
            {
                return BadRequest(new { Message = $"There is no anough: {product.Name} in stock. This is just {product.Quantity} pcs!" });
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
            if(product.Quantity == 0)
            {
                product.InStock = "No";
                _context.Update(product);
                _context.SaveChanges();
            }

            return Ok(new { Message = "You have successfully bought this product!" });
        }
        [Authorize]
        [HttpGet("GetMyPurchases")]
        public IActionResult GetMyPurchases()
        {
            var userId = int.Parse(User.Identity.Name);
            var myPurchases = _productService.GetMyPurchases(userId);

            if (myPurchases.Any())
            {
                return Ok(myPurchases);
            }
            else
            {
                return Ok(new { Message = "You do not have any purchase at the moment!" });
            }
        }
    }
}
