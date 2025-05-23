using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;
using ProductApi.Services;

namespace ProductApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public AuthController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(UserRegisterDto userDto)
    {
        try
        {
            var user = await _userRepository.Register(userDto);
            var response = await _userRepository.Login(new UserLoginDto 
            { 
                Username = userDto.Username,
                Password = userDto.Password!
            });
            
            return Ok(response);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(UserLoginDto userDto)
    {
        try
        {
            var response = await _userRepository.Login(userDto);
            return Ok(response);
        }
        catch (ApplicationException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}