using AuthApi.Interfaces;
using AuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var success = await _authService.RegisterAsync(request);
        return success ? Ok("User registered") : BadRequest("User already exists");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {   
        var token = await _authService.LoginAsync(request);
        return token == null ? Unauthorized("Invalid credentials.") : Ok(new { token });
    }

    [Authorize]
    [HttpPost]
    public IActionResult Logout()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        _authService.Logout(token);
        return Ok("Logged out");
    }
}

