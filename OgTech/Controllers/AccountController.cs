using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OgTech.Api.DTOs;
using OgTech.Api.Errors;
using OgTech.Core.Entities;
using OgTech.Core.Services;
using System.Security.Claims;

namespace OgTech.Controllers
{

    public class AccountController : ApiBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthService _authService;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,
                                 IAuthService authService,
                                 SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _authService = authService;
            _signInManager = signInManager;
        }




        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var userExisting = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExisting is not null)
            {
                return BadRequest(new ApiResponse(400, "This Account Already Exists"));
            }

            var user = new AppUser()
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email.Split('@')[0]
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "Failed to create, try later."));
            }
            var userDto = new UserDto()
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            };
            
            return Ok(userDto);
        }




        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
            {
                return NotFound(new ApiResponse(400, "You are unauthorized."));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return NotFound(new ApiResponse(400, "You are unauthorized."));
            }

            var userDto = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = loginDto.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            };

            return Ok(userDto);
        }


        [Authorize]
        [HttpDelete("Delete")]
        public async Task<ActionResult> Delete()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return NotFound(new ApiResponse(400, "User not Found."));
            }
            var resut = await _userManager.DeleteAsync(user);
            if (resut.Succeeded)
            {
                return StatusCode(StatusCodes.Status200OK, "Deleted Successfully!");
            }
            return BadRequest(new ApiResponse(400, "Error deleting this User"));
        }
    }
}
