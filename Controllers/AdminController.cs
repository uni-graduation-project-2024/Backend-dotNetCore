using Learntendo_backend.Data;
using Learntendo_backend.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Learntendo_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AdminController(IAuthRepository authRepository)
        {
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
    }
}
