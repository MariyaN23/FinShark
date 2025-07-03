using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService,
        SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var appUser = new AppUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Username,
            };

            var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (createdUser.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                return roleResult.Succeeded
                    ? Ok(
                        new NewUserDto
                        {
                            Username = appUser.UserName,
                            Email = appUser.Email,
                            Token = _tokenService.CreateToken(appUser)
                        })
                    : StatusCode(500, roleResult.Errors);
            }
            else
            {
                return StatusCode(500, createdUser.Errors);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());
        if (user == null) return Unauthorized("Invalid username");
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded) return Unauthorized("Username not found and/or password incorrect");
        return Ok(
            new NewUserDto
            {
                Username = user.UserName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            });
    }
}