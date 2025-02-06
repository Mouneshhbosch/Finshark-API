using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountsController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName);
                if (user == null)
                {
                    return Unauthorized("Invalid username");
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (!result.Succeeded)
                {
                    return Unauthorized("Invalid username or password.");
                }
                return Ok(new NewUserDto
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Token = _tokenService.CreateToken(user)
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (string.IsNullOrEmpty(registerDto.Password))
                {
                    return BadRequest("Password cannot be null or empty.");
                }
                var user = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);

                }
                var rolesCreated = await _userManager.AddToRolesAsync(user, ["User"]);
                if (!rolesCreated.Succeeded)
                {
                    return BadRequest(rolesCreated.Errors);
                }
                return Ok(new NewUserDto
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Token = _tokenService.CreateToken(user)
                }
                );
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }
    }
}