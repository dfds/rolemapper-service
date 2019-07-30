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

        public static async Task<FakeServerResponse> CallFakeServer(string endpoint, IServiceScope scope)
        {
            var response = await scope.ServiceProvider.GetRequiredService<HttpClient>()
                .GetAsync($"http://localhost:50901{endpoint}");
            var content = await response.Content.ReadAsStringAsync();
            return new JsonSerializer().Deserialize<FakeServerResponse>(content);
        }

        public static async Task ResetFakeServer(IServiceScope scope)
        {
            await CallFakeServer("/api-calls-reset", scope);
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
}