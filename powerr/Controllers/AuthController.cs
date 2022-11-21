
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using powerr.Api.Models.Entities.User;
using powerr.Models;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace powerr.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IConfiguration _configuration;

        public AuthController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }





        // GET: api/<AuthController>

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginUser)
        {
           

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                //get user from database by email
                var user = await _userManager.FindByEmailAsync(loginUser.Email);


                //if user exists and the provided password matches
                if(user != null && await _userManager.CheckPasswordAsync(user, loginUser.Password))
                {
                    //get user roles and do what ever with this information
                    var userRoles = await _userManager.GetRolesAsync(user);


                    //generate list of claims
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, loginUser.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach(var role in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                    }

                    var token = GetToken(authClaims);




                    return Ok(new
                    {
                        token = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }

                return StatusCode(StatusCodes.Status401Unauthorized, new
                {
                    status = "unauthorized",
                    message = "username or password is incorrect"
                });


            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

        }



        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel registrationModel)
        {
            try { 
            //check if user already exists
            var userExists = await _userManager.FindByEmailAsync(registrationModel.Email);

            if(userExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseModel
                {
                    status = "error",
                    message = "user already exists"
                });
            }


            AppUser user = new()
            {
                FirstName = registrationModel.FirstName,
                LastName = registrationModel.LastName,
                Email = registrationModel.Email,
                UserName = registrationModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),

            };

            var result = await _userManager.CreateAsync(user, registrationModel.Password);

            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseModel { status = "Error", message = "User creation failed" });
            };

            var roleExists =  await _roleManager.RoleExistsAsync("Customer");

            if (roleExists == false)
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            return Ok(new HttpResponseModel { status = "success", message = "User created Successfully" });

        }
            catch (Exception err)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = err.Message});
            }
        }


        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegistrationModel regModel)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(regModel.Email);

                if (userExists != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseModel { status = "error", message = "user already exists" });
                }

                //create instance of our app user
                AppUser user = new AppUser()
                {
                    Email = regModel.Email,
                    FirstName = regModel.FirstName,
                    LastName = regModel.LastName,
                    SecurityStamp = Guid.NewGuid().ToString(),

                };

                var result = await _userManager.CreateAsync(user, regModel.Password);

                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseModel
                    {
                        status = "errror",
                        message = "user creation failed"
                    });
                }

                var roleExists = _roleManager.RoleExistsAsync("Admin");

                if (roleExists != null)
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                await _userManager.AddToRoleAsync(user, "Admin");

                return Ok(new HttpResponseModel { status = "success", message = "Admin created Successfully" });

            }
            catch (Exception err)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = err.Message });
            }
         

        }

        private System.IdentityModel.Tokens.Jwt.JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            //gets secret from config and signs it.
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(

                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
