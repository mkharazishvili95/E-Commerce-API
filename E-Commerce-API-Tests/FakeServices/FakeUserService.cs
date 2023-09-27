using E_Commerce_API.Models;
using E_Commerce_API.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce_API_Tests.FakeServices
{
    public class FakeUserService : IUserService
    {
        private readonly List<User> _users;

        public FakeUserService(List<User> users)
        {
            _users = users;
        }

        public User Login(UserLoginModel loginModel)
        {
            return _users.FirstOrDefault(u => u.UserName == loginModel.UserName && u.Password == loginModel.Password);
        }

        public User Register(UserRegisterModel registerModel)
        {
            if (string.IsNullOrEmpty(registerModel.UserName) || string.IsNullOrEmpty(registerModel.Password))
            {
                return null;
            }

            var newUser = new User
            {
                Id = _users.Count + 1,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Age = registerModel.Age,
                Email = registerModel.Email,
                ShippingAddress = registerModel.ShippingAddress,
                UserName = registerModel.UserName,
                Password = registerModel.Password,
            };
            _users.Add(newUser);
            return newUser;
        }

        public double GetMyBalance(int userId)
        {
            var existingUser = _users.SingleOrDefault(x => x.Id == userId);
            if (existingUser == null)
            {
                throw new ArgumentException("User not found!");
            }

            return existingUser.Balance;
        }

        public User GetMyProfile(int userId)
        {
            var existingUser = _users.SingleOrDefault(x => x.Id == userId);
            if (existingUser == null)
            {
                throw new ArgumentException("User not found!");
            }

            return existingUser;
        }
    }
}
