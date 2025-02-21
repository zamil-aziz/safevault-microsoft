namespace SafeVault.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SafeVault.Models;
using SafeVault.Services;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserRepository _userRepository;
    private readonly AuthService _authService;

    public UsersController(UserRepository userRepository, AuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        try
        {
            var user = await _authService.RegisterUser(model.Username, model.Email, model.Password);
            return Ok(new { message = "User registered successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _authService.AuthenticateUser(model.Username, model.Password);
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials" });

        return Ok(new { message = "Login successful" });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsers();
        return Ok(users);
    }
}
