using iot_project.Data;
using iot_project.DTOs.IoT;
using iot_project.Enum;
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
        const string cacheKey = "cardCache";

        public IoTController(
            IIdentityCardRepository identityCardRepository,
            IMemoryCache cache,
            ICheckCardHistoryRepository checkCardHistoryRepository
        )
        {
            _cache = cache;
            _identityCardRepository = identityCardRepository;
            _checkCardHistoryRepository = checkCardHistoryRepository;
        }

        [HttpPost("register-user")]
        public IActionResult registerUser(RegisterUserDTO registerUserDTO)
        {
            IdentityCard cacheData = new IdentityCard
            {
                fullName = registerUserDTO.fullName,
                birthday = registerUserDTO.birthday,
                phone = registerUserDTO.phone,
            };
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            _cache.Set(cacheKey, cacheData, cacheOptions);
            return Ok(new {
                message = "Vui quẹt thẻ để đăng kí"
            });
        }


        [HttpGet("check-card/{IdCard}")]
        public IActionResult checkCard([FromRoute] string IdCard)
        {
            CheckCardStatus status = CheckCardStatus.EXIST;
            var identityCard = _identityCardRepository.getByIdCard(IdCard);
            if (identityCard == null)
            {
                if (_cache.TryGetValue<IdentityCard>(cacheKey, out var cacheData))
                {
                    IdentityCard newIdentityCard = cacheData as IdentityCard;
                    newIdentityCard.idCard = IdCard;
                    _identityCardRepository.create(newIdentityCard);
                    status = CheckCardStatus.CREATED;
                }
                else
                {
                    status = CheckCardStatus.UN_DEFINED;
                }
            }
            else
            {
                status = CheckCardStatus.EXIST;
            }
            _cache.Remove(cacheKey);
            var checkCardHistory = new CheckCardHistory
            {
                idCard = IdCard,
                status = status,
                time = DateTime.Now,
            };
            _checkCardHistoryRepository.create(checkCardHistory);
            return Ok(new
            {
                status = (int)status
            });
        }

        [HttpGet("history")]
        public IActionResult history()
        {
            List<CheckCardHistory> histories = _checkCardHistoryRepository.listOpen();
            string[] idCards = histories.Select(history => history.idCard).ToArray();

            List<IdentityCard> identityCards = _identityCardRepository.listByIdCards(idCards);

            var identityCardKeyById = _identityCardRepository.keyById(identityCards);
            return Ok(new {
                data = (new ListCheckCardHistoryTransformer()).transformFromList(histories, identityCardKeyById)
            });
        }
    }
}
