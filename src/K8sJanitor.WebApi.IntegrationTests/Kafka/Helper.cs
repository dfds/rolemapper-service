using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace K8sJanitor.WebApi.IntegrationTests.Kafka
{
    public class Helper
    {
        public static void SetEnvVars()
        {
            SetEnvVar("KUBERNETES_SERVICE_KAFKA_BOOTSTRAP_SERVERS", "localhost:9092");
            SetEnvVar("KUBERNETES_SERVICE_KAFKA_GROUP_ID", "kubernetes-consumer");
            SetEnvVar("KUBERNETES_SERVICE_KAFKA_ENABLE_AUTO_COMMIT", "false");
        }

        public static void SetEnvVar(string key, string val)
        {
            if (Environment.GetEnvironmentVariable(key) == null)
            {
                Environment.SetEnvironmentVariable(key, val);
            }
        }
        
        public static IConsumer<string, string> SetupKafkaConsumption(IServiceScope scope)
        {
            var consumerFactory = scope.ServiceProvider.GetRequiredService<KafkaConsumerFactory>();
            var eventRegistry = scope.ServiceProvider.GetRequiredService<DomainEventRegistry>();
            var topics = eventRegistry.GetAllTopics();
            var consumer = consumerFactory.Create();
            consumer.Subscribe(topics);

            return consumer;
        }
        
        public static IServiceProvider SetupServiceProviderWithConsumerAndProducer()
        {
            Helper.SetEnvVars();
            var eventRegistry = new DomainEventRegistry();
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            var publishingEventsQueue = new PublishingEventsQueue();

            var httpClient = new HttpClient();

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton(httpClient)
                .AddTransient<IEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>, GenericEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>>()
                .AddSingleton(eventRegistry)
                .AddSingleton(publishingEventsQueue)
                .AddTransient<KafkaConsumerFactory.KafkaConfiguration>()
                .AddTransient<KafkaPublisherFactory>()
                .AddTransient<KafkaConsumerFactory>()
                .AddTransient<IEventDispatcher, EventDispatcher>()
                .BuildServiceProvider();

            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole();

            return serviceProvider;
        }

        public static async Task<FakeServerResponse> CallFakeServer(string endpoint, IServiceScope scope)
        {
            var response = await scope.ServiceProvider.GetRequiredService<HttpClient>()
                .GetAsync($"http://localhost:50901{endpoint}");
            var content = await response.Content.ReadAsStringAsync();
            return new JsonSerializer().Deserialize<FakeServerResponse>(content);
        }
        
        public static async Task RunManualPublishingServiceOnce(IServiceScope scope)
        {
            var eventsQueue = scope.ServiceProvider.GetRequiredService<PublishingEventsQueue>();
            if (eventsQueue.AnyEventInQueue() == false)
            {
                return;
            }
                
            Console.WriteLine($"Domain events to publish: {eventsQueue.QueueCount()}");
                
            var publisherFactory = scope.ServiceProvider.GetRequiredService<KafkaPublisherFactory>();
            var eventRegistry = scope.ServiceProvider.GetRequiredService<DomainEventRegistry>();
                    
            Console.WriteLine("Connecting to kafka...");

            using (var producer = publisherFactory.Create())
            {
                Console.WriteLine("Connected!");

                while (eventsQueue.AnyEventInQueue())
                {
                    var evt = eventsQueue.Dequeue();
                    var topicName = eventRegistry.GetTopicFor(evt.Type);
                    var message = MessagingHelper.CreateMessageFrom(evt);

                    try
                    {
                        var result = await producer.ProduceAsync(
                            topic: topicName,
                            message: new Message<string, string>
                            {
                                Key = evt.AggregateId,
                                Value = message
                            }
                        );
                        Console.WriteLine($"Domain event \"{evt.Type}>{evt.EventId}\" has been published!");

                    }
                    catch (Exception)
                    {
                        throw new Exception($"Could not publish domain event \"{evt.Type}>{evt.EventId}\"!!!");

                    }
                }
            }
        }
    }
    
    public class GenericEventHandler<T> : IEventHandler<T>
    {
        public async Task HandleAsync(T domainEvent)
        {
            Console.WriteLine("GenericEventHandler called");
        }
    }

    public class FakeServerResponse
    {
        public bool Success { get; set; }
        public int ApiCallsReceived { get; set; }
    }
    
    public class JsonSerializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public JsonSerializer()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                Culture = CultureInfo.InvariantCulture,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
        }

        public string Serialize(object instance)
        {
            return JsonConvert.SerializeObject(
                value: instance,
                settings: _jsonSerializerSettings
            );
        }

        public T Deserialize<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public StringContent GetPayload(object objectToSerialize)
        {
            var payload = new StringContent(
                content: JsonConvert.SerializeObject(objectToSerialize, _jsonSerializerSettings),
                encoding: Encoding.UTF8,
                mediaType: "application/json"
            );

            return payload;
        }

        public T GetTokenValue<T>(string jsonInput, string path)
        {
            dynamic json = JValue.Parse(jsonInput);    
            var token = json.SelectToken(path);

            return token.ToObject<T>();
        }
    }
}