using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using StudentManagementSystem.Core.DTOs;
using StudentManagementSystem.Core.Interfaces;
using StudentManagementSystem.Infrastructure.Services;

namespace StudentManagementSystem.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _loggerMock = new Mock<ILogger<AuthService>>();
            _authService = new AuthService(
                _userRepositoryMock.Object,
                _jwtServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_ShouldSucceed()
        {
            // Arrange
            var registerDto = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "password123",
                ConfirmPassword = "password123",
                FullName = "New User"
            };

            _userRepositoryMock.Setup(repo => repo.UserExistsByEmailAsync(registerDto.Email))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(repo => repo.CreateUserAsync(registerDto.Email, It.IsAny<string>(), registerDto.FullName))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("User registered successfully", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_ExistingUser_ShouldFail()
        {
            // Arrange
            var registerDto = new RegisterRequestDto
            {
                Email = "existing@example.com",
                Password = "password123",
                ConfirmPassword = "password123",
                FullName = "Existing User"
            };

            _userRepositoryMock.Setup(repo => repo.UserExistsByEmailAsync(registerDto.Email))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User with this email already exists", result.Message);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ShouldSucceed()
        {
            // Arrange
            var loginDto = new LoginRequestDto
            {
                Email = "user@example.com",
                Password = "password123"
            };

            // Password hash for "password123"
            var passwordHash = "+hHdpH8/LCbLcXJBVI/um4IRnU7NX4WmD7fwfMXIE40=";

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync((true, passwordHash, "User Name"));
            _jwtServiceMock.Setup(service => service.GenerateToken(loginDto.Email, loginDto.Email))
                .Returns("valid-jwt-token");

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Login successful", result.Message);
            Assert.Equal("valid-jwt-token", result.Token);
        }

        [Fact]
        public async Task LoginAsync_InvalidEmail_ShouldFail()
        {
            // Arrange
            var loginDto = new LoginRequestDto
            {
                Email = "nonexistent@example.com",
                Password = "password123"
            };

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync((false, null, null));

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid email or password", result.Message);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ShouldFail()
        {
            // Arrange
            var loginDto = new LoginRequestDto
            {
                Email = "user@example.com",
                Password = "wrongpassword"
            };

            // Password hash for "password123"
            var passwordHash = "+hHdpH8/LCbLcXJBVI/um4IRnU7NX4WmD7fwfMXIE40=";

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync((true, passwordHash, "User Name"));

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid email or password", result.Message);
        }
    }
}
