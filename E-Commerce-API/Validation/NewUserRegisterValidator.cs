using E_Commerce_API.Data;
using E_Commerce_API.Models;
using FluentValidation;
using System.Linq;

namespace E_Commerce_API.Validation
{
    public class NewUserRegisterValidator : AbstractValidator<UserRegisterModel>
    {
        private readonly ECommerceContext _context;
        public NewUserRegisterValidator(ECommerceContext context) {
            _context = context;
            RuleFor(newUser => newUser.FirstName).NotEmpty().WithMessage("Enter your FirstName!");
            RuleFor(newUser => newUser.LastName).NotEmpty().WithMessage("Enter your LastName!");
            RuleFor(newUser => newUser.Age).NotNull().NotEmpty().WithMessage("Enter your Age!")
                .GreaterThanOrEqualTo(18).WithMessage("Minimal Age must be 18 in order to register!");
            RuleFor(newUser => newUser.UserName).NotEmpty().WithMessage("Enter your UserName!")
                .Length(6, 15).WithMessage("UserName length must be between 6 and 15 chars or numbers!")
                .Must(differentUserName).WithMessage("UserName already exists. Try another!");
            RuleFor(newUser => newUser.Password).NotEmpty().WithMessage("Enter your Password!")
                .Length(6, 15).WithMessage("Password length must be between 6 and 15 chars or numbers!");
            RuleFor(newUser => newUser.Email).NotEmpty().WithMessage("Enter your E-Mail address!")
                .EmailAddress().WithMessage("Enter your Valid E-Mail address!")
                .Must(differentEmail).WithMessage("E-Mail already exists. Try another!");
            RuleFor(newUser => newUser.ShippingAddress).NotEmpty().WithMessage("Enter your Shipping Address!");
        }
        private bool differentUserName(string userName)
        {
            var different = _context.Users.SingleOrDefault(existingUser => existingUser.UserName.ToUpper() == userName.ToUpper());
            return different == null;
        }
        private bool differentEmail(string eMail)
        {
            var differentEmailAddress = _context.Users.SingleOrDefault(existingUser => existingUser.Email.ToUpper() == eMail.ToUpper());
            return differentEmailAddress == null;
        }
    }
}
