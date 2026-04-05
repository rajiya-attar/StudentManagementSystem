using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using StudentManagementSystem.Infrastructure.Services;

namespace StudentManagementSystem.Tests.Services
{
    public class JwtServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<JwtService>> _loggerMock;
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<JwtService>>();

            // Setup configuration section
            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(s => s["SecretKey"]).Returns("YourSuperSecretKeyForJWTTokenGeneration2024!@#$%^&*()_+");
            jwtSectionMock.Setup(s => s["Issuer"]).Returns("TestIssuer");
            jwtSectionMock.Setup(s => s["Audience"]).Returns("TestAudience");
            jwtSectionMock.Setup(s => s["ExpirationMinutes"]).Returns("60");

            _configurationMock.Setup(c => c.GetSection("JwtSettings"))
                .Returns(jwtSectionMock.Object);

            _jwtService = new JwtService(_configurationMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GenerateToken_WithValidData_ShouldReturnToken()
        {
            // Arrange
            var email = "test@example.com";
            var userId = "123";

            // Act
            var token = _jwtService.GenerateToken(email, userId);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            Assert.True(token.Split('.').Length == 3); // JWT has 3 parts
        }

        [Fact]
        public void GenerateToken_WithNullSecretKey_ShouldThrowException()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(s => s["SecretKey"]).Returns((string?)null);

            configMock.Setup(c => c.GetSection("JwtSettings"))
                .Returns(jwtSectionMock.Object);

            var jwtService = new JwtService(configMock.Object, _loggerMock.Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => jwtService.GenerateToken("test@example.com", "123"));
        }
    }
}
