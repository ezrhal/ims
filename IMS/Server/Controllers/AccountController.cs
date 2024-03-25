using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using IMS.Server.Models.Identity;
using IMS.Shared.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IMS.Server.Services;
using Microsoft.AspNetCore.Authorization;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        //private readonly SignInManager<MongoIdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        IMongoDBService _db;

        public AccountController(IConfiguration configuration, IMongoDBService db)
        {
           // _signInManager = signInManager;
            _configuration = configuration;
            _db = db;
            
        }

        public async Task<IActionResult> Login([FromBody]LoginModel login)
        {
            //var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);

            var result = await _db.CheckUserCredential(login.Username, login.Password);

            if (!result)
                return BadRequest(new LoginResult { Successful = false, Error = "Invalid Username or Password" });

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, login.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
                claims,
                expires: expiry,
                signingCredentials: creds
            );

            return Ok(new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });

        }

        public async Task<string> Login1(LoginModel login)
        {
            //var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);

            var result = await _db.CheckUserCredential(login.Username, login.Password);

            if (!result)
                return "Bad Request"; //BadRequest(new LoginResult { Successful = false, Error = result.ToString() });

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenkey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("KEY1312312312RANDOMARISHISOLUTIONSAMEOLDSHIRTKEY1312312312RANDOMARISHISOLUTIONSAMEOLDSHIRT"));
            var signingCreds = new SigningCredentials(tokenkey, SecurityAlgorithms.HmacSha256Signature);

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, login.Username) }),
                SigningCredentials = signingCreds,
                Expires = DateTime.UtcNow.AddDays(7),
            });

            //var tokenDescriptor = new SecurityTokenDescriptor()
            //{
            //    Subject = new ClaimsIdentity(new Claim[]
            //    {
            //        new Claim(ClaimTypes.Name, login.Username)
            //    }),
            //    Expires = DateTime.UtcNow.AddHours(1),
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256Signature)

            //};

            //var token = tokenHandler.CreateToken(tokenDescriptor);

            //return Ok(new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });

            return tokenHandler.WriteToken(token);
        }

        [Authorize]
        [HttpPost]
        public async Task<string> CheckAuth()
        {
            return "string";
        }
    }
}

