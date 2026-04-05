using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentManagementSystem.Core.Interfaces;
using StudentManagementSystem.Infrastructure.Data;

namespace StudentManagementSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email && u.IsActive);
        }

        public async Task<bool> CreateUserAsync(string email, string passwordHash, string fullName)
        {
            try
            {
                var user = new Core.Models.User
                {
                    Email = email,
                    PasswordHash = passwordHash,
                    FullName = fullName,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email: {Email}", email);
                throw; // Rethrow to see actual error
            }
        }

        public async Task<(bool exists, string? passwordHash, string? fullName)> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .Where(u => u.Email == email && u.IsActive)
                .Select(u => new { u.PasswordHash, u.FullName })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return (false, null, null);
            }

            return (true, user.PasswordHash, user.FullName);
        }
    }
}
