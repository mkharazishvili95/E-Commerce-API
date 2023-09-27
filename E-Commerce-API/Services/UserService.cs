using E_Commerce_API.Data;
using E_Commerce_API.Helpers;
using E_Commerce_API.Models;
using E_Commerce_API.Validation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace E_Commerce_API.Services
{
    public interface IUserService
    {
        User Register(UserRegisterModel registerModel);
        User Login(UserLoginModel loginModel);
        double GetMyBalance(int userId);
        public User GetMyProfile(int userId);
    }
    public class UserService : IUserService
    {
        private readonly ECommerceContext _context;
        public UserService(ECommerceContext context)
        {
            _context = context;
        }

        public User GetMyProfile(int userId)
        {
            var existingUser = _context.Users.SingleOrDefault(x => x.Id == userId);
            if (existingUser == null)
            {
                throw new ArgumentException("User not found!");
            }

            return existingUser;
        }

        public double GetMyBalance(int userId)
        {
            var existingUser = _context.Users.SingleOrDefault(x => x.Id == userId);
            if (existingUser == null)
            {
                throw new ArgumentException("User not found!");
            }

            return existingUser.Balance;
        }
        public User Login(UserLoginModel loginModel)
        {
            if (string.IsNullOrEmpty(loginModel.UserName) || string.IsNullOrEmpty(loginModel.Password) ||
                string.IsNullOrEmpty(loginModel.Email))
            {
                return null;
            }
            var user = _context.Users.FirstOrDefault(x => x.UserName == loginModel.UserName);
            if (user == null)
            {
                return null;
            }
            if (HashSettings.HashPassword(loginModel.Password) != user.Password)
            {
                return null;
            }
            if(loginModel.Email != user.Email)
            {
                return null;
            }
            _context.SaveChanges();
            return user;
        }

        public User Register(UserRegisterModel registerModel)
        {
            var userValidator = new NewUserRegisterValidator(_context);
            var validatorResult = userValidator.Validate(registerModel);
            if (!validatorResult.IsValid)
            {
                return null;
            }
            else
            {
                var newUser = new User
                {
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    Email = registerModel.Email,
                    UserName = registerModel.UserName,
                    Password = HashSettings.HashPassword(registerModel.Password),
                    Age = registerModel.Age,

                };
                _context.Add(newUser);
                _context.SaveChanges();
                return (newUser);
            }
        }
    }
}
