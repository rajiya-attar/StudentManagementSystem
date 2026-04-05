using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Core.DTOs;
using StudentManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace StudentManagementSystem.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] RegisterRequestDto request)
        {
            _logger.LogInformation("Processing registration for user: {Email}", request.Email);

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Invalid registration data"));
            }

            try
            {
                var result = await _authService.RegisterAsync(request);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse(result.Message));
                }

                return Ok(ApiResponse<string>.SuccessResponse(null, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user: {Email}", request.Email);
                return StatusCode(500, ApiResponse<string>.ErrorResponse("An error occurred during registration"));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            _logger.LogInformation("Processing login for user: {Email}", request.Email);

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid login data"));
            }

            try
            {
                var result = await _authService.LoginAsync(request);

                if (!result.Success)
                {
                    return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse(result.Message));
                }

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                    new AuthResponseDto { Token = result.Token },
                    "Login successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Email}", request.Email);
                return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("An error occurred during login"));
            }
        }
    }
}
