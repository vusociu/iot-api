using iot_project.Data;
using iot_project.DTOs.IoT;
using iot_project.Enum;
using iot_project.Helpers;
using iot_project.Models;
using iot_project.Transformer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace iot_project.Controllers
{
    [Route("iot/api")]
    [ApiController]
    public class IoTController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IIdentityCardRepository _identityCardRepository;
        private readonly ICheckCardHistoryRepository _checkCardHistoryRepository;
        private readonly MqttService _mqttService;
        const string cacheKey = "cardCache";

        public IoTController(
            IIdentityCardRepository identityCardRepository,
            IMemoryCache cache,
            ICheckCardHistoryRepository checkCardHistoryRepository,
            MqttService mqttService
        )
        {
            _cache = cache;
            _identityCardRepository = identityCardRepository;
            _checkCardHistoryRepository = checkCardHistoryRepository;
            _mqttService = mqttService;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> registerUserAsync(RegisterUserDTO registerUserDTO)
        {
            DateTime birthDay = DateTime.ParseExact(registerUserDTO.birthday, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            IdentityCard cacheData = new IdentityCard
            {
                fullName = registerUserDTO.fullName,
                birthday = birthDay,
                phone = registerUserDTO.phone,
            };
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            _cache.Set(cacheKey, cacheData, cacheOptions);
            _mqttService.PublishAsync(MqttTopic.SERVER, "register");
            return Ok(new {
                message = "Success"
            });
        }

        [HttpGet("history")]
        public IActionResult history()
        {
            List<CheckCardHistory> histories = _checkCardHistoryRepository.listOpen();
            return Ok(new {
                data = (new ListCheckCardHistoryTransformer()).transformFromList(histories)
            });
        }

        [HttpGet("open-door")]
        public IActionResult openDoor()
        {
            _mqttService.PublishAsync(MqttTopic.SERVER, "open");
            return Ok(new
            {
                message = "Success"
            });
        }
    }
}
