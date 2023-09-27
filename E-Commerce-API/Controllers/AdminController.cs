using E_Commerce_API.Data;
using E_Commerce_API.Models;
using E_Commerce_API.Services;
using E_Commerce_API.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ECommerceContext _context;
        public AdminController(IAdminService adminService, ECommerceContext context)
        {
            _adminService = adminService;
            _context = context;
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpGet("GetAllUsers")]
        public IEnumerable<User> GetAllUsers()
        {
            var userList = _adminService.GetAllUsers();
            return userList;
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpGet("GetUserById")]
        public IActionResult GetUserById(int userId)
        {
            var existingUser = _context.Users.SingleOrDefault(x => x.Id == userId);
            if(existingUser == null)
            {
                return NotFound(new {Message = "There is no any User by this ID!"});
            }
            else
            {
                return Ok(_adminService.GetUserById(userId));
            }
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpGet("SortUsersByAddress")]
        public IActionResult SortUsersByAddress(string address)
        {
            var getSortedList = _context.Users.Where(x => x.ShippingAddress.ToUpper().Contains(address.ToUpper())).ToList();
            var sortedUsersByCountry = _adminService.SortUsersByAddress(address);
            if(sortedUsersByCountry == null) 
            {
                return NotFound(new { Message = "There is no any User from this Country!" });
            }
            else
            {
                return Ok(getSortedList);
            }
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPut("BlockUser")]
        public IActionResult BlockUser(int userId)
        {
            var existingUser = _context.Users.SingleOrDefault(user => user.Id == userId);
            if(existingUser == null)
            {
                return BadRequest(new { Message = "There is no any User by this ID to block!" });
            }else if(existingUser.IsBlocked == true)
            {
                return BadRequest(new { Message = "That User is already Blocked!" });
            }
            if(existingUser.Role == Roles.Admin)
            {
                return BadRequest(new { Message = "You have no permission to block another Admin!" });
            }
            else
            {
                _adminService.BlockUser(userId);
                return Ok(new { Message = "User has successfully blocked!" });
            }
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPut("UnblockUser")]
        public IActionResult UnblockUser(int userId)
        {
            var existingUser = _context.Users.SingleOrDefault(user => user.Id == userId);
            if(existingUser == null)
            {
                return BadRequest(new { Message = "There is no any User by this ID to unblock!" });
            }else if(existingUser.IsBlocked == false)
            {
                return BadRequest(new { Message = "That User is already Unblocked!" });
            }
            else
            {
                _adminService.UnblockUser(userId);
                return Ok(new { Message = "User has successfully unblocked!" });
            }
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPost("AddProduct")]
        public IActionResult AddProduct(Product newProduct)
        {
            var productValidator = new ProductValidator(_context);
            var validatorResult = productValidator.Validate(newProduct);
            if (!validatorResult.IsValid)
            {
                return BadRequest(validatorResult.Errors);
            }
            else
            {
                _adminService.AddProduct(newProduct);
                return Ok(new { Message = $"Product: {newProduct.Name} - ({newProduct.Quantity} pcs) has successfully added!" });
            }
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPut("UpdateProduct")]
        public IActionResult UpdateProduct(Product updateProduct, int productId)
        {
            var existingProduct = _context.Products.SingleOrDefault(product => product.Id == productId);
            if(existingProduct == null)
            {
                return NotFound(new {Message = "There is no any product by this ID to update!"});
            }
            else
            {
                var productValidator = new ProductValidator(_context);
                var validatorResult = productValidator.Validate(updateProduct);
                if (!validatorResult.IsValid)
                {
                    return BadRequest(validatorResult.Errors);
                }
                else
                {
                    _adminService.UpdateProduct(updateProduct, productId);
                    return Ok(new { Message = "Product has successfully updated!" });
                }
            }
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("DeleteProduct")]
        public IActionResult DeleteProduct(int productId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                var existingProduct = _context.Products.SingleOrDefault(product => product.Id == productId);

                if (existingProduct == null)
                {
                    return BadRequest(new { Message = "There is no any product by this ID to delete!" });
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
                return Ok(new { Message = $"Product: {existingProduct.Name} has successfully deleted!" });
            }
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("PurchaseCancellation")]
        public IActionResult PurchaseCancellation(int purchaseId)
        {
            var purchase = _context.Purchases.FirstOrDefault(x => x.Id == purchaseId);

            if (purchase == null)
            {
                return BadRequest(new { Message = "Purchase not found!" });
            }
            var product = _context.Products.FirstOrDefault(x => x.Id == purchase.ProductId);
            var user = _context.Users.FirstOrDefault(x => x.Id == purchase.UserId);
            var receiverBank = _context.BankModels.FirstOrDefault() ?? new BankModel();
            if (product == null || user == null || receiverBank == null)
            {
                return BadRequest(new { Message = "Error!" });
            }
            var paymentCalculator = (product.Price * purchase.Quantity);
            user.Balance += paymentCalculator;
            receiverBank.BankStatement -= paymentCalculator;
            product.Quantity += purchase.Quantity;
            _context.Update(user);
            _context.Update(receiverBank);
            _context.Update(product);
            if (product.Quantity > 0 && product.InStock == "No")
            {
                product.InStock = "Yes";
                _context.Update(product);
            }
            _context.Remove(purchase);
            _context.SaveChanges();
            return Ok(new { Message = "Purchase has been successfully cancelled!" });
        }
    }
}
