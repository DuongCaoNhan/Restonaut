using APIs.Entities.Interfaces;
using APIs.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<User>>> GetActiveUsers()
        {
            var users = await _unitOfWork.Users.GetActiveUsersAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(CreateUserRequest request)
        {
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _unitOfWork.Users.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }

    public class CreateUserRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    public class UpdateUserRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}