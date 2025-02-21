using Microsoft.EntityFrameworkCore;
using SafeVault.Data;
using SafeVault.Models;

namespace SafeVault.Services;

public class UserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserById(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> GetUserByUsername(string username)
    {
        if (!InputValidator.ValidateInput(username))
            throw new ArgumentException("Invalid username");

        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _context.Users.ToListAsync();
    }
}
