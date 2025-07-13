using APIs.DTOs;
using APIs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IJwtTokenService jwtTokenService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
        }        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(UserRegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid model state"
                });
            }

            var user = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }

            var token = _jwtTokenService.GenerateToken(user.Id, user.Email!);
            var expiryMinutes = Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"]);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "User registered successfully",
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                UserId = user.Id,
                Email = user.Email
            });
        }        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid model state"
                });
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid credentials"
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid credentials"
                });
            }

            var token = _jwtTokenService.GenerateToken(user.Id, user.Email!);
            var expiryMinutes = Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"]);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                UserId = user.Id,
                Email = user.Email
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public ActionResult<AuthResponse> Logout(LogoutRequest request)
        {
            // For JWT tokens, logout is typically handled client-side by simply discarding the token
            // However, you can implement token blacklisting or other server-side logout mechanisms here
            
            // Note: SignInManager.SignOutAsync() is mainly for cookie-based authentication
            // For JWT, we just return success since the token will be discarded client-side

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Logout successful"
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<AuthResponse>> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "User retrieved successfully",
                UserId = user.Id,
                Email = user.Email
            });
        }

        [HttpPost("refresh")]
        [Authorize]
        public ActionResult<AuthResponse> RefreshToken()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (userId == null || email == null)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var token = _jwtTokenService.GenerateToken(userId, email);
            var expiryMinutes = Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"]);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Token refreshed successfully",
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                UserId = userId,
                Email = email
            });
        }
    }
}