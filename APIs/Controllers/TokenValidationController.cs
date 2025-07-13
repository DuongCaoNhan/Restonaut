using APIs.DTOs;
using APIs.Services;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenValidationController : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService;

        public TokenValidationController(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("validate")]
        public ActionResult<AuthResponse> ValidateToken([FromBody] TokenValidationRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Token is required"
                });
            }

            var principal = _jwtTokenService.ValidateToken(request.Token);
            if (principal == null)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var userId = principal.GetUserId();
            var email = principal.GetEmail();
            var expiration = principal.GetExpirationTime();

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Token is valid",
                UserId = userId,
                Email = email,
                ExpiresAt = expiration
            });
        }

        [HttpGet("info")]
        public ActionResult<AuthResponse> GetTokenInfo()
        {
            // Extract token from the Authorization header
            var token = HttpContext.Request.ExtractTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Authorization header with Bearer token is required"
                });
            }

            var principal = _jwtTokenService.ValidateToken(token);
            if (principal == null)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var userId = principal.GetUserId();
            var email = principal.GetEmail();
            var expiration = principal.GetExpirationTime();

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Token info retrieved successfully",
                UserId = userId,
                Email = email,
                ExpiresAt = expiration
            });
        }
    }

    public class TokenValidationRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}