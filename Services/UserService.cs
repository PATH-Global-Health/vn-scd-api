using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Data.Constants;
using Data.Entities;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Services.RabbitMQ;

namespace Services
{
    public interface IUserService
    {
        Task<ResultModel> SignUp(UserCreateModel model);
        Task<ResultModel> Login(LoginModel login);
    }
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly IProducer _producer;

        public UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, SignInManager<User> signInManager, IProducer producer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _producer = producer;
        }

        public async Task<ResultModel> SignUp(UserCreateModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                //First Step: Create UserCreateModel with props:

                //public string Username { get; set; }
                //public string Password { get; set; }
                //public string Email { get; set; } = "";
                //public string PhoneNumber { get; set; } = "";
                //public string FullName { get; set; } = "";

                //Get UserCreateModel data from controller
                //Parse object to string
                //Send request to RabbitMQ by : _producer.CreateAccount("REGISTER OBJECT");
                //Return result or error message

                string message = JsonConvert.SerializeObject(model);
                string rs =_producer.CreateAccount(message);
                if (rs.Contains("not working or too busy"))
                {
                    result.ErrorMessage = rs;
                    return result;
                }


                var existUserName = _userManager.Users.FirstOrDefault(user => user.UserName == model.UserName);
                if (existUserName != null) throw new Exception("This Username already existed");
                var user = new User
                {
                    UserName = model.UserName,
                    PhoneNumber = model.PhoneNumber,
                    FullName = model.FullName
                };

                bool isExistedRole = await _roleManager.RoleExistsAsync(SystemRoles.Customer);

                if (!isExistedRole)
                {
                    var role = new IdentityRole { Name = SystemRoles.Customer };
                    await _roleManager.CreateAsync(role);
                }
                var create = await _userManager.CreateAsync(user, model.Password);

                if (create.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, SystemRoles.Customer);
                    result.Data = user.UserName;
                    result.Succeed = true;
                }
                else
                {
                    throw new Exception(string.Join("\n", create.Errors.Select(s => s.Description)));
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public async Task<ResultModel> Login(LoginModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                var signInResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

                if (!signInResult.Succeeded) throw new Exception("Invalid username or password");

                var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == model.UserName);
                var roles = await _userManager.GetRolesAsync(appUser);

                object token = GenerateJwtToken(appUser, roles[0]);

                var successViewModel = new LoginSuccessViewModel()
                {
                    Token = token,
                    Role = roles[0],
                    UserId = appUser.Id
                };

                result.Data = successViewModel;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public object GenerateJwtToken(IdentityUser user, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["JwtIssuerOptions:Issuer"],
                _configuration["JwtIssuerOptions:Audience"],
                claims,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
