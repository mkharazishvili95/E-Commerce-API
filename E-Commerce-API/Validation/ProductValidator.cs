using E_Commerce_API.Data;
using E_Commerce_API.Models;
using FluentValidation;
using Microsoft.Data.SqlClient.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace E_Commerce_API.Validation
{
    public class ProductValidator : AbstractValidator<Product>
    {
        private readonly ECommerceContext _context;
        public ProductValidator(ECommerceContext context)
        {
            _context = context;
            RuleFor(newProduct => newProduct.Name).NotEmpty().WithMessage("Enter Product Name!");
            RuleFor(newProduct => newProduct.Description).NotEmpty().WithMessage("Enter Product Description!");
            RuleFor(newProduct => newProduct.Quantity).NotEmpty().WithMessage("Enter Product Quantity!");
            RuleFor(newProduct => newProduct.ProductCategory).NotEmpty().WithMessage("Enter Product Category!")
                .Must(category => category == Category.Accessories || category == Category.Flowers ||
                category == Category.HomeAppliances).WithMessage("You can only choose one product category from them: " +
                $"{Category.HomeAppliances}, {Category.Flowers}, {Category.Accessories}");
            RuleFor(newProduct => newProduct.InStock).NotEmpty().WithMessage("Is product available now? Yes/No")
                .Must(x => x.Contains("Yes") || x.Contains("No")).WithMessage("Is product In Stock? Enter Yes/No");
            RuleFor(newProduct => newProduct.Price).NotEmpty().WithMessage("Enter Product's price! If it's" +
                "not available now enter 0!");
        }
    }
}

