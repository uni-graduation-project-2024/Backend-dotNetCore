using Learntendo_backend.Data;
using Learntendo_backend.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Learntendo_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context; 
        private readonly IAuthRepository _authRepository;

        public AdminController(DataContext context, IAuthRepository authRepository)
        {
            _context = context;
            _authRepository = authRepository;
        }

        [HttpPost("Adminlogin")]
        public async Task<IActionResult> LoginAdmin(LoginDto loginDto)
        {
            var userInData = await _authRepository.LoginAdmin(loginDto.Email, loginDto.Password);
            if (userInData == null)
                return Unauthorized();

            return Ok();
        }

        [HttpGet("view-users")]
        public async Task<IActionResult> ViewUsers()
        {
            var users = await _context.User.ToListAsync();

         
            if (users.Count == 0)
            {
                return Ok(new { message = "No users found." });
            }

            return Ok(users);
        }

        
        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.User.FindAsync(userId);

           
            if (user == null)
            {
                return Ok(new { message = "User not found." });
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully!" });
        }
    }
}

