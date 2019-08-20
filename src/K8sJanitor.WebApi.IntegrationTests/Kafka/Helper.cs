using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Infrastructure.Messaging;
using K8sJanitor.WebApi.Repositories;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Services;
using K8sJanitor.WebApi.Validators;
using K8sJanitor.WebApi.Wrappers;
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

        public static Services SetupV2()
        {
            return new Services();
        }
        
        public static IServiceProvider SetupServiceProviderWithConsumerAndProducer(bool useManualEvents = false)
        {
            Helper.SetEnvVars();
            var eventRegistry = new DomainEventRegistry();
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            var publishingEventsQueue = new PublishingEventsQueue();

            var httpClient = new HttpClient();

            var serviceProviderBuilder = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton(httpClient)
//                .AddTransient<IConfigMapService, ConfigMapService>()
//                .AddTransient<IAwsAuthConfigMapRepository, AwsAuthConfigMapRepository>()
//                .AddTransient<IAddRoleRequestValidator, AddRoleRequestValidator>()
//                .AddTransient<IAddNamespaceRequestValidator, AddNamespaceRequestValidator>()
//                .AddTransient<IKubernetesWrapper>(k => new KubernetesWrapper(null))
//                .AddTransient<IPersistenceRepository, PersistenceRepositoryStub>()
//                .AddTransient<INamespaceRepository, NamespaceRepository>()
//                .AddTransient<IRoleRepository, RoleRepository>()
//                .AddTransient<IRoleBindingRepository, RoleBindingRepository>()
                .AddTransient<IEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>, GenericEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>>()
                .AddSingleton(eventRegistry)
                .AddSingleton(publishingEventsQueue)
                .AddTransient<KafkaConsumerFactory.KafkaConfiguration>()
                .AddTransient<KafkaPublisherFactory>()
                .AddTransient<KafkaConsumerFactory>()
                .AddTransient<IEventDispatcher, EventDispatcher>();

            if (useManualEvents)
            {
                ManualEvents.AddEventsToServiceProvider(serviceProviderBuilder);
            }
            
            var serviceProvider = serviceProviderBuilder.BuildServiceProvider();

            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole();

            return serviceProvider;
        }

        public static async Task<FakeServerResponse> CallFakeServer(string endpoint, Services services)
        {
            var response = await services.HttpClient
                .GetAsync($"http://localhost:50901{endpoint}");
            var content = await response.Content.ReadAsStringAsync();
            return new JsonSerializer().Deserialize<FakeServerResponse>(content);
        }

        public static async Task ResetFakeServer(Services services)
        {
            await CallFakeServer("/api-calls-reset", services);
        }

        public static async Task<FakeServerResponse> PostFakeServer(string endpoint, IServiceScope scope, HttpContent content)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"http://localhost:50901{endpoint}"),
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/json"}
                },
                Content = content
            };

            var response = await scope.ServiceProvider.GetRequiredService<HttpClient>()
                .SendAsync(request);
            
            var contentResp = await response.Content.ReadAsStringAsync();
            return new JsonSerializer().Deserialize<FakeServerResponse>(contentResp);
        }
 
        public static async Task<List<Type>> GetAllEventTypes()
        {
            var listOfIEvents = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => ( typeof(IEvent).IsAssignableFrom(x) ) && !x.IsInterface && !x.IsAbstract)
                .Select(x => x).ToList();
            
            var listOfDomainEvents = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x =>
                {
                    var interfaces = x.GetInterfaces();
                    
                    foreach (var interf in interfaces)
                    {
                        if (interf.Name.Contains("IDomainEvent") && interf.Namespace.Equals("K8sJanitor.WebApi.Domain.Events"))
                        {
                            return true;
                        }
                    }

                    return false;
                })
                .Where(x => !x.Name.Equals("GeneralDomainEvent"))
                .Select(x => x).ToList();

            var allEvents = new List<Type>();
            allEvents.AddRange(listOfIEvents);
            allEvents.AddRange(listOfDomainEvents);
            return allEvents;
        }

        public static Configuration GetConfiguration()
        {
            var test = new ConfigurationTest();
            test.InitViaEnvVars();
            var kafka = new ConfigurationKafka();
            kafka.InitViaEnvVars();

            return new Configuration(test, kafka);
        }

        public static IConsumer<string, string> CreateKafkaConsumer()
        {
            var config = new ConsumerConfig();
            var builder = new ConsumerBuilder<string, string>(config);
            builder.SetErrorHandler(OnKafkaError);
            return builder.Build();
        }

        private static void CreateKafkaConf()
        {
            const string KEY_PREFIX = "KUBERNETES_SERVICE_KAFKA_";
            
            var configurationKeys = new[]
            {
                "group.id",
                "enable.auto.commit",
                "bootstrap.servers",
                "broker.version.fallback",
                "api.version.fallback.ms",
                "ssl.ca.location",
                "sasl.username",
                "sasl.password",
                "sasl.mechanisms",
                "security.protocol",
            };

            var config = configurationKeys
                .Select(key =>
                {
                    var val = Environment.GetEnvironmentVariable(key);
                });
        }
        
        private static void OnKafkaError(IConsumer<string, string> producer, Error error)
        {
            if (error.IsFatal)
                Environment.FailFast($"Fatal error in Kafka producer: {error.Reason}. Shutting down...");
            else
                throw new Exception(error.Reason);
        }

        public static async Task<List<object>> GetPrefilledEvents()
        {
            var payload = new List<object>();

            return payload;
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

    public class FakeServerPostRequest
    {
        public string NamespaceName { get; set; }
        public string ContextId { get; set; }
        public string CapabilityId { get; set; }
    }

    public class FakeServerResponse
    {
        public bool Success { get; set; }
        public int ApiCallsReceived { get; set; }
        public int KafkaMessageReceived { get; set; }
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

    public class Services
    {
        public HttpClient HttpClient { get; private set; }

        public Services()
        {
            HttpClient = new HttpClient();
        }
    }

    public class Configuration
    {
        public ConfigurationTest Test { get; private set; }
        public ConfigurationKafka Kafka { get; private set; }

        public Configuration(ConfigurationTest test, ConfigurationKafka kafka)
        {
            Test = test;
            Kafka = kafka;
        }
    }

    public interface IConfigurationFunctionality
    {
        void InitViaEnvVars();
    }

    public class ConfigurationTest : IConfigurationFunctionality
    {
        public bool Run { get; set; }
        public string Topic { get; set; }
        public string FakeServerHost { get; set; }
        public string PayloadDir { get; set; }
        
        public void InitViaEnvVars()
        {
            const string PREFIX = "INTEGRATION_TEST_KAFKA";
            
            Run = Environment.GetEnvironmentVariable($"{PREFIX}_RUN") != null 
                ? Boolean.Parse(Environment.GetEnvironmentVariable($"{PREFIX}_RUN") ?? throw new NullReferenceException("Unable to convert ENV VAR to bool")) 
                : true;
            Topic = Environment.GetEnvironmentVariable($"{PREFIX}_TOPIC") != null
                ? Environment.GetEnvironmentVariable($"{PREFIX}_TOPIC")
                : "build.capabilities";
            FakeServerHost = Environment.GetEnvironmentVariable($"{PREFIX}_FAKE-SERVER_HOST") != null
                ? Environment.GetEnvironmentVariable($"{PREFIX}_FAKE-SERVER_HOST")
                : "localhost:50901";
            PayloadDir = Environment.GetEnvironmentVariable($"{PREFIX}_PAYLOAD_DIR") != null
                ? Environment.GetEnvironmentVariable($"{PREFIX}_PAYLOAD_DIR")
                : "TODO";
        }
    }

    public class ConfigurationKafka : IConfigurationFunctionality
    {
        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
        public bool EnableAutoCommit { get; set; }
        public void InitViaEnvVars()
        {
            const string PREFIX = "KUBERNETES_SERVICE_KAFKA";

            BootstrapServers = Environment.GetEnvironmentVariable($"{PREFIX}_BOOTSTRAP_SERVERS") != null
                ? Environment.GetEnvironmentVariable($"{PREFIX}_BOOTSTRAP_SERVERS")
                : "localhost:9092";
            GroupId = Environment.GetEnvironmentVariable($"{PREFIX}_GROUP_ID") != null
                ? Environment.GetEnvironmentVariable($"{PREFIX}_GROUP_ID")
                : "kubernetes-consumer";
            EnableAutoCommit = Environment.GetEnvironmentVariable($"{PREFIX}_ENABLE_AUTO_COMMIT") != null
                ? Boolean.Parse(Environment.GetEnvironmentVariable($"{PREFIX}_ENABLE_AUTO_COMMIT") ?? throw new NullReferenceException("Unable to convert ENV VAR to bool"))
                : false;
        }
    }
}