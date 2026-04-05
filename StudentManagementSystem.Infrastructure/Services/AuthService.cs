using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using StudentManagementSystem.Core.DTOs;
using StudentManagementSystem.Core.Interfaces;

namespace StudentManagementSystem.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IJwtService jwtService, ILogger<AuthService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequestDto request)
        {
            _logger.LogInformation("Processing registration for user: {Email}", request.Email);

            // Check if user already exists
            if (await _userRepository.UserExistsByEmailAsync(request.Email))
            {
                _logger.LogWarning("User with email {Email} already exists", request.Email);
                return new AuthResult
                {
                    Success = false,
                    Message = "User with this email already exists"
                };
            }

            // Hash password
            var passwordHash = HashPassword(request.Password);

            // Create user
            var success = await _userRepository.CreateUserAsync(request.Email, passwordHash, request.FullName);

            if (!success)
            {
                _logger.LogError("Failed to create user: {Email}", request.Email);
                return new AuthResult
                {
                    Success = false,
                    Message = "Failed to create user. Please try again."
                };
            }

            _logger.LogInformation("User registered successfully: {Email}", request.Email);
            return new AuthResult
            {
                Success = true,
                Message = "User registered successfully"
            };
        }

        public async Task<AuthResult> LoginAsync(LoginRequestDto request)
        {
            _logger.LogInformation("Processing login for user: {Email}", request.Email);

            // Get user by email
            var (exists, passwordHash, fullName) = await _userRepository.GetUserByEmailAsync(request.Email);

            if (!exists)
            {
                _logger.LogWarning("Login attempt failed - user not found: {Email}", request.Email);
                return new AuthResult
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Verify password
            if (!VerifyPassword(request.Password, passwordHash!))
            {
                _logger.LogWarning("Login attempt failed - invalid password: {Email}", request.Email);
                return new AuthResult
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(request.Email, request.Email);
            var expiration = DateTime.UtcNow.AddMinutes(60);

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);
            return new AuthResult
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                Expiration = expiration
            };
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private static bool VerifyPassword(string password, string passwordHash)
        {
            var hash = HashPassword(password);
            return hash == passwordHash;
        }
    }
}
