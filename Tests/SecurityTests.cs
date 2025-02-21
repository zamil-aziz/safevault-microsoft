using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeVault.Data;
using SafeVault.Services;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SafeVault.Data;
using SafeVault.Models;
using SafeVault.Services;
using SafeVault.Authorization;

namespace SafeVault.Tests;

public class SecurityTests
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;
    private readonly UserRepository _userRepository;

    public SecurityTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        _context = new AppDbContext(options);
        var passwordHasher = new PasswordHasher<User>();
        _authService = new AuthService(_context, passwordHasher);
        _userRepository = new UserRepository(_context);
    }

    [Fact]
    public async Task TestSQLInjectionPrevention()
    {
        var maliciousUsername = "admin'; DROP TABLE Users; --";
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _userRepository.GetUserByUsername(maliciousUsername));
    }

    [Fact]
    public void TestXSSPrevention()
    {
        var maliciousInput = "<script>alert('xss')</script>";
        Assert.False(InputValidator.ValidateInput(maliciousInput));
    }

    [Fact]
    public async Task TestPasswordHashing()
    {
        var username = "testuser";
        var email = "test@example.com";
        var password = "SecurePass123!";

        var user = await _authService.RegisterUser(username, email, password);
        Assert.NotEqual(password, user.PasswordHash);
    }

    [Fact]
    public async Task TestAuthorizationCheck()
    {
        var username = "adminuser";
        var email = "admin@example.com";
        var password = "AdminPass123!";

        var user = await _authService.RegisterUser(username, email, password);
        user.Role = "Admin";
        await _context.SaveChangesAsync();

        var handler = new RoleAuthorizationHandler();
        var requirement = new RoleRequirement("Admin");
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Admin") });
        var principal = new ClaimsPrincipal(identity);
        var context = new AuthorizationHandlerContext(new[] { requirement }, principal, null);

        await handler.HandleAsync(context);
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public void TestEmailValidation()
    {
        Assert.True(InputValidator.ValidateEmail("valid@email.com"));
        Assert.False(InputValidator.ValidateEmail("invalid-email"));
    }
}
