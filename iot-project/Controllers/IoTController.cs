using iot_project.DTOs.IoT;
using iot_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace iot_project.Controllers
{
    [Route("iot/api")]
    [ApiController]

    public class IoTController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public IoTController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpPost("register-user")]
        IActionResult registerUser(RegisterUserDTO registerUserDTO)
        {
            const string cacheKey = "cardCache";
            IdentityCard cacheData = new IdentityCard
            {
                fullName = registerUserDTO.fullName,
                birthday = registerUserDTO.birthday,
                phone = registerUserDTO.phone,
            };
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            _cache.Set(cacheKey, cacheData, cacheOptions);
            return Ok("Success");
        }
    }
}
