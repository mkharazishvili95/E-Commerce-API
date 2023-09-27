using System;
using System.Collections.Generic;
using E_Commerce_API.Models;
using E_Commerce_API.Services;
using E_Commerce_API_Tests.FakeServices;
using NUnit.Framework;

namespace E_Commerce_API_Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private IUserService _userService;
        [SetUp]
        public void Setup()
        {
            var users = new List<User>
            {
                new User { Id = 1, UserName = "Misho999", Password = "Misho999", Email = "Misho999@gmail.com" },
                new User { Id = 2, UserName = "Admin123", Password = "Admin123", Email = "Admin123@gmail.com"}
            };

            _userService = new FakeUserService(users);
        }
        [Test]
        public void Login_ValidCredentials_ReturnsUser()
        {
            var user = _userService.Login(new UserLoginModel { UserName = "Misho999", Password = "Misho999", Email = "Misho999@gmail.com" });

            Assert.IsNotNull(user);
            Assert.AreEqual(1, user.Id);
        }

        [Test]
        public void Login_InvalidCredentials_ReturnsNull()
        {
            var user = _userService.Login(new UserLoginModel { UserName = "Misho999", Password = "IncorrectPasswordForTesting", Email = "Misho999@gmail.com" });

            Assert.IsNull(user);
        }
        [Test]
        public void Register_InvalidData_ReturnsNull()
        {

            var invalidRegisterModel = new UserRegisterModel
            {
                FirstName = "Misho",
                LastName = "Kharazishvili",
                UserName = "Misho999",
                Age = 28,
                ShippingAddress = "Georgia, Tbilisi",
                Email = "IncorrectE-MailAddressForTesting",
                Password = ""//Password must contain (Example just for testing it!)
            };

            var result = _userService.Register(invalidRegisterModel);

            Assert.IsNull(result);
        }
        [Test]
        public void Register_ValidData_ReturnsUser()
        {
            var validRegisterModel = new UserRegisterModel
            {
                FirstName = "Misho",
                LastName = "Kharazishvili",
                UserName = "Misho999",
                Age = 28,
                ShippingAddress = "Georgia, Tbilisi",
                Email = "Misho999@gmail.com",
                Password = "Misho999"
            };

            var result = _userService.Register(validRegisterModel);

            Assert.IsNotNull(result);
            Assert.AreEqual(validRegisterModel.FirstName, result.FirstName);
            Assert.AreEqual(validRegisterModel.LastName, result.LastName);
            Assert.AreEqual(validRegisterModel.UserName, result.UserName);

        }
        [Test]
        public void GetMyProfile_ValidUserId_ReturnsUser()
        {
            List<User> users = new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "Misho",
                    LastName = "Kharazishvili",
                    UserName = "Misho999",
                    Password = "Misho999",
                    Balance = 100.00,
                    Email = "Misho999@gmail.com",
                    ShippingAddress = "Georgia, Tbilisi",
                    Age = 28
                }
            };
            IUserService userService = new FakeUserService(users);
            User result = userService.GetMyProfile(1);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Misho", result.FirstName);
            Assert.AreEqual("Kharazishvili", result.LastName);
            Assert.AreEqual("Misho999", result.UserName);
            Assert.AreEqual(100.00, result.Balance);
            Assert.AreEqual("Misho999@gmail.com", result.Email);
            Assert.AreEqual("Georgia, Tbilisi", result.ShippingAddress);
            Assert.AreEqual(28, result.Age);
        }

        [Test]
        public void GetMyProfile_InvalidUserId_ThrowsException()
        {
            List<User> users = new List<User>();
            IUserService userService = new FakeUserService(users);
            Assert.Throws<ArgumentException>(() => userService.GetMyProfile(1));
        }

        [Test]
        public void GetMyBalance_ValidUserId_ReturnsBalance()
        {
            List<User> users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Balance = 100.00
                }
            };
            IUserService userService = new FakeUserService(users);
            double result = userService.GetMyBalance(1);
            Assert.AreEqual(100.00, result);
        }

        [Test]
        public void GetMyBalance_InvalidUserId_ThrowsException()
        {
            List<User> users = new List<User>();
            IUserService userService = new FakeUserService(users);
            Assert.Throws<ArgumentException>(() => userService.GetMyBalance(1));
        }
    }
}
