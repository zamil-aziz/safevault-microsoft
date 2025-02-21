using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeVault.Data;
using SafeVault.Models;

namespace SafeVault.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private AppDbContext context;
    private PasswordHasher<User> passwordHasher;

    public AuthService(AppDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public AuthService(AppDbContext context, PasswordHasher<User> passwordHasher)
    {
        this.context = context;
        this.passwordHasher = passwordHasher;
    }

    public async Task<User> RegisterUser(string username, string email, string password)
    {
        if (!InputValidator.ValidateInput(username) || !InputValidator.ValidateEmail(email))
            throw new ArgumentException("Invalid input");

        var user = new User
        {
            Username = username,
            Email = email,
            Role = "User"
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> AuthenticateUser(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null) return null;

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success ? user : null;
    }
}
