using System;
using Xunit;
using Testogsikkerhed_CICD.Controller;
using Testogsikkerhed_CICD.Services;
using Testogsikkerhed_CICD.Helpers;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Testogsikkerhed_CICD.Dtos;
using Microsoft.EntityFrameworkCore;
using Testogsikkerhed_CICD.DataAccess;
using Testogsikkerhed_CICD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Testogsikkerhed_CICD.Tests
{
    public class UsersControllerTests
    {
        private readonly UsersController _usersController;
        private readonly IUserService _userService;
        public UsersControllerTests()
        {
            var appSettings = new AppSettings()
            {
                Secret = "Secret string used to verify JWT tokens"
            };
            IOptions<AppSettings> appSettingsOptions = Options.Create(appSettings);

            // Name our in memory database w/ a GUID to ensure that its a fresh database every time.
            var contextOptions = new DbContextOptionsBuilder<TestContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            IContext testContext = new TestContext(contextOptions);
            _userService = new UserService(testContext);
            _usersController = new UsersController(_userService, appSettingsOptions);
            _usersController.ControllerContext.HttpContext = new DefaultHttpContext();

            // Populate the database w/ test data.
            UserRegisterDto[] userDtos = new UserRegisterDto[]
{
                new UserRegisterDto { Name = "One", Password = "onepw", Email = "one@gmail.com" },
                new UserRegisterDto { Name = "Two", Password = "twopw", Email = "two@something.com" }
};
            foreach (var userDto in userDtos)
            {
                User u = new User()
                {
                    Name = userDto.Name,
                    Email = userDto.Email
                };
                _userService.Create(u, userDto.Password);
            }
        }

        [Fact]
        public async Task GetUser_AuthorizedUserShouldWork()
        {
            // Arrange authorization.

            // Set the id of the user via claims.
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, "1")
            };
            _usersController.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act.
            var user = (await _usersController.GetUser(1)).Value;

            // Asserts that it includes values that only authorized users get access to.
            Assert.True(!string.IsNullOrEmpty(user.Email));
        }

        [Fact]
        public async Task GetUser_UnauthorizedUserShouldWork()
        {
            // Arrange authorization.

            // Set the id of the user via claims.
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, "1")
            };
            _usersController.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act.
            var user = (await _usersController.GetUser(1)).Value;

            // Asserts that it doesn't includes values that only authorized users get access to.
            Assert.True(!string.IsNullOrEmpty(user.Email));
        }

        [Fact]
        public async Task GetUser_InvalidUserShouldReturnNotFound()
        {
            // Asserts that a NotFoundResult is returned, when getting a non-existant user.
            Assert.True((await _usersController.GetUser(int.MaxValue)).Result is NotFoundResult);
        }

        [Fact]
        public async Task PostUser_CreationOfSameNameShoudNotWork()
        {
            var userDto = new UserRegisterDto()
            {
                Name = "TestUser",
                Password = "pw",
                Email = "TestUser@mail.com"
            };

            await _usersController.PostUser(userDto);
            Assert.True((await _usersController.PostUser(userDto)) is BadRequestObjectResult);
        }
    }
}
