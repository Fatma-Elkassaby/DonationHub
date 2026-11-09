using DonationHub.Application.Features.Authentication.CreateStaff;
using DonationHub.Application.Features.Authentication.GetCurrentUser;
using DonationHub.Application.Features.Authentication.Login;
using DonationHub.Application.Features.Authentication.Logout;
using DonationHub.Application.Features.Authentication.RefreshToken;
using DonationHub.Application.Features.Authentication.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace DonationHub.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly RegisterService _registerService;
        private readonly LoginService _loginService;
        private readonly RefreshTokenService _refreshTokenService;
        private readonly LogoutService _logoutService;
        private readonly GetCurrentUserService _getCurrentUserService;
        private readonly CreateStaffService _createStaffService;

        public AuthController(
            RegisterService registerService,
            LoginService loginService,
            RefreshTokenService refreshTokenService,
            LogoutService logoutService,
            GetCurrentUserService getCurrentUserService,
            CreateStaffService createStaffService)
        {
            _registerService = registerService;
            _loginService = loginService;
            _refreshTokenService = refreshTokenService;
            _logoutService = logoutService;
            _getCurrentUserService = getCurrentUserService;
            _createStaffService = createStaffService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _registerService.RegisterAsync(request);
            if (result.ErrorCode == "Auth.EmailAlreadyExists")
                return Conflict(new { Error = result.ErrorCode });
            if (result.ErrorCode is not null)
            {
                return BadRequest(new
                {
                    Error = result.ErrorCode,
                    Details = result.Errors
                });
            }
            return StatusCode(
                StatusCodes.Status201Created,
                result.Response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            LoginRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _loginService.LoginAsync(
                request,
                cancellationToken);
            if (result.ErrorCode == "Auth.InvalidCredentials")
                return Unauthorized(new { Error = result.ErrorCode });
            if (result.ErrorCode == "Auth.AccountDeactivated")
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { Error = result.ErrorCode });
            if (result.ErrorCode is not null)
                return BadRequest(new { Error = result.ErrorCode });
            return Ok(result.Response);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(
            RefreshTokenRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _refreshTokenService.RefreshAsync(
                request,
                cancellationToken);
            if (result.ErrorCode == "Auth.RefreshTokenInvalid")
                return Unauthorized(new { Error = result.ErrorCode });
            if (result.ErrorCode == "Auth.AccountDeactivated")
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { Error = result.ErrorCode });
            if (result.ErrorCode is not null)
                return BadRequest(new { Error = result.ErrorCode });
            return Ok(result.Response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(
            LogoutRequest request,
            CancellationToken cancellationToken)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdValue, out var userId))
                return Unauthorized();
            var errorCode = await _logoutService.LogoutAsync(
                userId,
                request,
                cancellationToken);
            if (errorCode == "Auth.RefreshTokenInvalid")
                return Unauthorized(new { Error = errorCode });
            if (errorCode is not null)
                return BadRequest(new { Error = errorCode });
            return NoContent();
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdValue, out var userId))
                return Unauthorized();
            var user = await _getCurrentUserService.GetCurrentUserAsync(userId);
            if (user is null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost("~/api/v1/admin/staff")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStaff(CreateStaffRequest request)
        {
            var result = await _createStaffService.CreateStaffAsync(request);
            if (result.ErrorCode == "Auth.EmailAlreadyExists")
                return Conflict(new { Error = result.ErrorCode });
            if (result.ErrorCode == "Auth.RoleAssignmentFailed" ||
                result.ErrorCode == "Auth.RoleAssignmentAndCleanupFailed")
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        Error = result.ErrorCode,
                        Details = result.Errors
                    });
            }
            if (result.ErrorCode is not null)
            {
                return BadRequest(new
                {
                    Error = result.ErrorCode,
                    Details = result.Errors
                });
            }
            return StatusCode(
                StatusCodes.Status201Created,
                result.Response);
        }
    }
}
