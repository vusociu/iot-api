using iot_project.Data;
using iot_project.Enum;
using iot_project.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Win32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace iot_project.Helpers
{
    public class MqttSubcriberService: BackgroundService
    {
        private readonly MqttService _mqttService;
        private readonly ILogger<MqttSubcriberService> _logger;
        private readonly IMemoryCache _cache;
        private readonly IServiceProvider _serviceProvider;

        public MqttSubcriberService(
            MqttService mqttService,
            ILogger<MqttSubcriberService> logger,
            IMemoryCache cache,
            IServiceProvider serviceProvider
        )
        {
            _mqttService = mqttService;
            _logger = logger;
            _cache = cache;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var topicHandlers = new Dictionary<string, Func<string, string, Task>>
                {
                    [MqttTopic.DEVICE_REGISTER] = async (topic, message) =>
                    {
                        var scope = _serviceProvider.CreateScope();
                        var identityCardRepository = scope.ServiceProvider.GetRequiredService<IIdentityCardRepository>();
                        string cacheKey = "cardCache";
                       
                        if (!string.IsNullOrEmpty(message))
                        {
                            if (message == "no card registered" || message== "No response from server.")
                            {
                                _cache.Remove(cacheKey);
                            }
                            else
                            {
                                string idCard = message;
                                var identityCard = identityCardRepository.getByIdCard(idCard);
                                if (_cache.TryGetValue<IdentityCard>(cacheKey, out var cacheData))
                                {
                                    IdentityCard newIdentityCard = cacheData as IdentityCard;
                                    newIdentityCard.idCard = message.ToString();
                                    if (identityCard == null)
                                    {
                                        identityCardRepository.create(newIdentityCard);
                                        _mqttService.PublishAsync(MqttTopic.SERVER, "register done");
                                    }
                                    else
                                    {
                                        identityCardRepository.updateIdCard(identityCard.id, newIdentityCard);
                                        _mqttService.PublishAsync(MqttTopic.SERVER, "overwrite register");
                                    }
                                    _cache.Remove(cacheKey);
                                }
                                else
                                {
                                    _mqttService.PublishAsync(MqttTopic.SERVER, "register fail");
                                }
                                
                            }
                        }
                        await Task.CompletedTask;
                    },
                    [MqttTopic.OPEN_DOOR] = async (topic, idCard) =>
                    {
                        if (!string.IsNullOrEmpty(idCard))
                        {
                            var status = CheckCardStatus.EXIST;
                            var scope = _serviceProvider.CreateScope();
                            var identityCardRepository = scope.ServiceProvider.GetRequiredService<IIdentityCardRepository>();
                            var checkCardHistoryRepository = scope.ServiceProvider.GetRequiredService<ICheckCardHistoryRepository>();
                            var identityCard = identityCardRepository.getByIdCard(idCard);
                            if (identityCard == null)
                            {
                                status = CheckCardStatus.UN_DEFINED;
                                _mqttService.PublishAsync(MqttTopic.SERVER, "deny");
                            }
                            else
                            {
                                _mqttService.PublishAsync(MqttTopic.SERVER, "open");
                            }
                            var checkCardHistory = new CheckCardHistory
                            {
                                idCard = idCard,
                                status = status,
                                time = DateTime.Now,
                                fullName = identityCard?.fullName ?? ""
                            };
                            checkCardHistoryRepository.create(checkCardHistory);
                        }
                        await Task.CompletedTask;
                    },
                };
                await _mqttService.SubscribeMultipleWithHandlersAsync(topicHandlers, cancellationToken: stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken); // Idle delay to prevent a tight loop
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("MqttSubscriberService is stopping due to cancellation.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in MqttSubscriberService.");
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MqttSubscriberService is stopping...");
            await _mqttService.DisconnectAsync(stoppingToken);
            await base.StopAsync(stoppingToken);
        }
    }
}
