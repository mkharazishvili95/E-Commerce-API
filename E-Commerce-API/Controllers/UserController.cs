using E_Commerce_API.Data;
using E_Commerce_API.Helpers;
using E_Commerce_API.Models;
using E_Commerce_API.Services;
using E_Commerce_API.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;
        private readonly ECommerceContext _context;
        public UserController(IUserService userService, AppSettings appSettings, ECommerceContext context)
        {
            _userService = userService;
            _appSettings = appSettings;
            _context = context;
        }
        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserRegisterModel newUser)
        {
            var validator = new NewUserRegisterValidator(_context);
            var validatorResult = validator.Validate(newUser);
            if (!validatorResult.IsValid)
            {
                return BadRequest(validatorResult.Errors);
            }
            else
            {
                var user = newUser;
                _userService.Register(user);
                return Ok(new { Message = $"User: {user.UserName} has successfully registered!" });
            }
        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserLoginModel login)
        {
            var userLogin = _userService.Login(login);
            if (userLogin == null)
            {
                return BadRequest(new { message = "Username, Password or E-Mail is incorrect!" });
            }
            else
            {
                Loggs log = new Loggs
                {
                    UserLogged = $"User {userLogin.Email} with Username: {userLogin.UserName} logged in.",
                    LoggDate = DateTime.Now
                };
                _context.Loggs.Add(log);
                _context.SaveChanges();


                var tokenString = GenerateToken(userLogin);

                return Ok(new
                {
                    Message = "You have successfully Logged!",
                    UserName = userLogin.UserName,
                    Email = userLogin.Email,
                    Role = userLogin.Role,
                    Token = tokenString
                });
            }
        }
        [Authorize]
        [HttpGet("GetMyBalance")]
        public IActionResult GetMyBalance()
        {
            var userId = int.Parse(User.Identity.Name);
            try
            {
                var balance = _userService.GetMyBalance(userId);
                return Ok(new { Message = $"You have {balance}GEL on your account!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("GetMyProfile")]
        public IActionResult GetMyProfile()
        {
            var userId = int.Parse(User.Identity.Name);
            try
            {
                var user = _userService.GetMyProfile(userId);
                return Ok(new
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Balance = $"{user.Balance}GEL",
                    ShippingAddress = user.ShippingAddress
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        private object GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.UserName.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(365),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}
