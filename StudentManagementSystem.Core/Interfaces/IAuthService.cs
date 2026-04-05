using StudentManagementSystem.Core.DTOs;

namespace StudentManagementSystem.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterRequestDto request);
        Task<AuthResult> LoginAsync(LoginRequestDto request);
    }

    public interface IUserRepository
    {
        Task<bool> UserExistsByEmailAsync(string email);
        Task<bool> CreateUserAsync(string email, string passwordHash, string fullName);
        Task<(bool exists, string? passwordHash, string? fullName)> GetUserByEmailAsync(string email);
    }
}
