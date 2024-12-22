using MQTTnet.Client;
using MQTTnet;
using System;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using MQTTnet.Protocol;
using System.Collections.Concurrent;

namespace iot_project.Helpers
{
    public class MqttService : IDisposable
    {
        private readonly IMqttClient _client;
        private readonly MqttClientOptions _options;
        private bool _disposed;

        private readonly ConcurrentDictionary<string, Func<string, string, Task>> _topicHandlers;

        public event Func<string, string, Task> OnMessageReceived;

        public bool IsConnected => _client?.IsConnected ?? false;

        public MqttService(string server, int port, string username, string password)
        {
            var mqttFactory = new MqttFactory();
            _client = mqttFactory.CreateMqttClient();
            _topicHandlers = new ConcurrentDictionary<string, Func<string, string, Task>>();

            _options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(server, port)
                 .WithCredentials(username, password)
                .WithCleanSession()
                .Build();

            SetupHandlers();
        }

        private void SetupHandlers()
        {
            _client.DisconnectedAsync += async e =>
            {
                Console.WriteLine($"Disconnected from MQTT broker: {e.Reason}");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await ConnectAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Reconnection failed: {ex.Message}");
                }
            };

            _client.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

                try
                {
                    if (_topicHandlers.TryGetValue(topic, out var handler))
                    {
                        await handler(topic, payload);
                    }

                    if (OnMessageReceived != null)
                    {
                        await OnMessageReceived(topic, payload);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            };
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (!_client.IsConnected)
                {
                    await _client.ConnectAsync(_options, cancellationToken);
                    Console.WriteLine("Connected to MQTT broker successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                throw;
            }
        }

        public async Task PublishAsync(
            string topic,
            string payload,
            bool retain = false,
            MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce,
            CancellationToken cancellationToken = default)
        {
            if (!IsConnected)
                await ConnectAsync(cancellationToken);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(qos)
                .WithRetainFlag(retain)
                .Build();

            await _client.PublishAsync(message, cancellationToken);
        }

        public async Task SubscribeMultipleWithHandlersAsync(
            Dictionary<string, Func<string, string, Task>> topicHandlers,
            MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce,
            CancellationToken cancellationToken = default)
        {
            if (!IsConnected)
                await ConnectAsync(cancellationToken);
            foreach (var handler in topicHandlers)
            {
                _topicHandlers.AddOrUpdate(handler.Key, handler.Value, (_, _) => handler.Value);
            }
            var subscribeOptionsBuilder = new MqttClientSubscribeOptionsBuilder();
            foreach (var topic in topicHandlers.Keys)
            {
                subscribeOptionsBuilder.WithTopicFilter(topicFilter =>
                {
                    topicFilter.WithTopic(topic).WithQualityOfServiceLevel(qos);
                });
            }
            var subscribeOptions = subscribeOptionsBuilder.Build();

            await _client.SubscribeAsync(subscribeOptions, cancellationToken);
        }

        public async Task UnsubscribeAsync(string[] topics, CancellationToken cancellationToken = default)
        {
            if (IsConnected)
            {
                foreach (var topic in topics)
                {
                    _topicHandlers.TryRemove(topic, out _);
                }

                var unsubscribeOptionsBuilder = new MqttClientUnsubscribeOptionsBuilder();

                foreach (var topic in topics)
                {
                    unsubscribeOptionsBuilder.WithTopicFilter(topic);
                }

                var unsubscribeOptions = unsubscribeOptionsBuilder.Build();

                await _client.UnsubscribeAsync(unsubscribeOptions, cancellationToken);
            }
        }


        public void RemoveTopicHandler(string topic)
        {
            _topicHandlers.TryRemove(topic, out _);
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            if (IsConnected)
            {
                await _client.DisconnectAsync(new MqttClientDisconnectOptions(), cancellationToken);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _client?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}