using BaseballCalcASP.Data;
using BaseballCalcASP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BaseballCalcASP.APIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Route("api/Account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(BaseballCalcASPContext context, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<bool>> Login([FromBody] APIModels.LoginModel model)
        {
            // Check if the user exists based on the email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Return a specific error if the user is not found
                return Ok(new { isAuthenticated = false, message = "Email not found" });
            }

            // Validate the password
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
            //var result = await _signInManager.PasswordSignInAsync("admin@testemail.com", "Start123#", false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                // Create a JWT token
                var token = CreateJwtToken(user);

                // Return authenticated together with the JWT token
                return Ok(new { isAuthenticated = true, token});
            }

            // Return a specific error if the password is incorrect
            return Ok(new { isAuthenticated = false, message = "Invalid password" });
        }
        private string CreateJwtToken(AppUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
