using iot_project.Data;
using iot_project.DTOs.Authenticate;
using iot_project.Helpers;
using iot_project.Models;
using Microsoft.AspNetCore.Mvc;

namespace iot_project.Controllers
{
    [Route("iot/api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly JwtService _jwtService;
        public AuthController(IUserRepository repository, JwtService jwtService)
        {
            _repository = repository;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public IActionResult register(RegisterDTO dto)
        {
            var user = new User
            {
                email = dto.email,
                password = BCrypt.Net.BCrypt.HashPassword(dto.password),
                fullName = dto.fullName,
                role = dto.role
            };
            return Created("success", _repository.create(user));
        }

        [HttpPost("login")]
        public IActionResult login(LoginDTO dto)
        {
            var user = _repository.getAdminByEmail(dto.email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid credentials" });
            }
            if (!BCrypt.Net.BCrypt.Verify(dto.password, user.password))
            {
                return BadRequest(new { message = "Invalid credentials" });
            }
            var jwt = _jwtService.generate(user.id);

            Response.Cookies.Append("bearer_token", jwt, new CookieOptions
            {
                HttpOnly = true
            });
            return Ok(new
            {
                token = jwt,
                fullName = user.fullName,
                role = user.role.ToString(),
            });
        }
    }
}
