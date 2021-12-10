using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Testogsikkerhed_CICD.Dtos;
using Testogsikkerhed_CICD.Exceptions;
using Testogsikkerhed_CICD.Helpers;
using Testogsikkerhed_CICD.Models;
using Testogsikkerhed_CICD.Services;

namespace Testogsikkerhed_CICD.Controller
{
    // Set the authorization scheme to Jwt Bearer - I.e. use a Jwt bearer token.
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;

        public UsersController(IUserService userService, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
            //DefaultData defaultData = new DefaultData(userService);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = await _userService.GetAll();
            UserDto[] userDtos = new UserDto[users.Count()];
            int i = 0;
            foreach (var user in users)
            {
                userDtos[i] = new UserDto()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Age = user.Age,
                    Email = user.Email
                };
                i++;
            }
            return userDtos;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {
                User user = await _userService.GetOne(id);
                UserDto userDto = new UserDto()
                {
                    Id = user.Id,
                    Age = user.Age,
                    Name = user.Name
                };

                // If authorized as that user, also give private info.
                if (id == Convert.ToInt32(User.Identity.Name))
                    userDto.Email = user.Email;

                return userDto;
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostUser(UserRegisterDto userDto)
        {
            try
            {
                User u = new()
                {
                    Id = userDto.Id,
                    Name = userDto.Name,
                    Age = userDto.Age,
                    Email = userDto.Email
                };
                await _userService.Create(u, userDto.Password);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(UserAuthDto userDto)
        {
            User user;
            try
            {
                user = await _userService.Authenticate(userDto.Name, userDto.Password);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            //Return basic user info and token to store on the client side.
            return Ok(new
            {
                user.Id,
                user.Name,
                Token = tokenString
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.Delete(id);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            return Accepted();
        }
    }
}
