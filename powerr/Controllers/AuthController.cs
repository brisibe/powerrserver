
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using powerr.Api.Models.Entities.User;
using powerr.Enums;
using powerr.Interfaces;
using powerr.Models;
using powerr.Models.Dtos;
using powerr.Models.Entities.Wallet;
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

        private readonly IWalletRepository _walletRepository;

        private readonly IConfiguration _configuration;

        public AuthController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IWalletRepository walletRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _walletRepository = walletRepository;

        }



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


                    //generate list of claims
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, loginUser.Email),
                        //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    //get user roles and do what ever with this information
                    var userRoles = await _userManager.GetRolesAsync(user);

                    foreach(var role in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var token = GetToken(authClaims);

                    //user type 1 for admin, 2 for customer

                    var usertype = userRoles.Count > 0 &&  userRoles[0] == Roles.Admin ? UserTypeEnum.ADMIN : UserTypeEnum.CUSTOMER;


                    return Ok(new
                    {   
                        id = user.Id,
                        email= user.Email,
                        firstName = user.FirstName,
                        userType = usertype,
                        token = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token),
                        //role = userRoles
                        //expiration = token.ValidTo
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

                return StatusCode(StatusCodes.Status400BadRequest, ModelState );
            };

           
               
            await _userManager.AddToRoleAsync(user, Roles.Customer);
                

                //Create Wallet for each registered customer
                var wallet = new Wallet
                {
                    Balance = 0,
                    UserId = user.Id,
                };

                 _walletRepository.Create(wallet);
                _walletRepository.Save();

                

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
                    UserName = regModel.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),

                };

                var result = await _userManager.CreateAsync(user, regModel.Password);

                if (!result.Succeeded)
                {

                    foreach (var error in result.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }

                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

            

                await _userManager.AddToRoleAsync(user, Roles.Admin);


                return Ok(new HttpResponseModel { status = "success", message = "Admin created Successfully" });

            }
            catch (Exception err)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = err.Message });
            }
         

        }

        [HttpGet]
        [Route("allCustomer")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                IList<UserDto> usersDto = new List<UserDto>();
              var users = await  _userManager.Users.ToListAsync();

                foreach (var user in users)
                {
                    usersDto.Add(new UserDto { Email = user.Email, FirstName = user.FirstName, Id = user.Id, LastName = user.LastName });

                }

                return Ok(new { statusCode = 200, message= "success", data = usersDto
                });
            }
            catch (Exception ex)
            {

               return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
