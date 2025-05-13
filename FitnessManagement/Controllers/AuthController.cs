using Fitness.Business.Abstract;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using FitnessManagement.Dtos;
using FitnessManagement.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
       
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAdminService _adminService;

        public AuthController(IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IAdminService adminService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _adminService = adminService;
        }

        [HttpPost("signup-admin")]
        public async Task<IActionResult> SignUpAdmin([FromBody] RegisterModelAdmin dto)
        {
            var admin = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                IsApproved = true 
            };

            var result = await _userManager.CreateAsync(admin, dto.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                await _userManager.AddToRoleAsync(admin, "Admin");
                await _adminService.AddAdminAsync(admin);
                return Ok(new { Status = "Success", Message = "Admin created successfully!" });
            }

            return BadRequest(new { Status = "Error", Message = "Admin creation failed!", Errors = result.Errors });
        }
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] RegisterModel dto)
        {
           
            if (dto.Role != "User" && dto.Role != "Trainer")
            {
                return BadRequest(new { Status = "Error", Message = "Invalid role! Only 'User' and 'Trainer' are allowed." });
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                IsApproved = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { Status = "Error", Message = "Registration failed!", Errors = result.Errors });
            }

            if (!await _roleManager.RoleExistsAsync(dto.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(dto.Role));
            }

            await _userManager.AddToRoleAsync(user, dto.Role);

            return Ok(new { Status = "Success", Message = $"User registered successfully as {dto.Role}! Waiting for admin approval." });
        }



        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] LoginModel dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return Unauthorized(new { Status = "Error", Message = "Invalid username or password!" });
            }

            if (!user.IsApproved)
            {
                return Unauthorized(new { Status = "Error", Message = "Your account is not approved yet. Please wait for admin approval." });
            }
            if (user.IsBlocked == true)
            {
                return Unauthorized(new { Status = "Error", Message = "Your account is blocked due to delayed payment. Please contact the administrator." });
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = GetToken(authClaims);
            var userRole=userRoles.FirstOrDefault();

            return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token), Expiration = token.ValidTo,Role=userRole });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}



   