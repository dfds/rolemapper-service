using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public class PublishingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private Queue<DomainEventEnvelope> _eventsToBeSent;

        public PublishingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _eventsToBeSent = new Queue<DomainEventEnvelope>();
        }

        public void AddEventToQueue(DomainEventEnvelope iEvent)
        {
            _eventsToBeSent.Enqueue(iEvent);
        }

        public void AddEventsToQueue(IEnumerable<DomainEventEnvelope> events)
        {
            foreach (DomainEventEnvelope @event in events)
            {
                _eventsToBeSent.Enqueue(@event);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Starting event publisher.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DoTestEvent(stoppingToken); // Used for learning purposes. REMOVE BEFORE MERGING TO MASTER
                    await DoWork(stoppingToken);
                }
                catch (Exception err)
                {
                    Log.Error(err, "Error processing and/or publishing domain events to message broker.");
                }

                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }

        // Entirely for learning purposes. 
        // REMOVE BEFORE MERGING TO MASTER.
        private async Task DoTestEvent(CancellationToken stoppingToken)
        {
            Log.Information("Sending test event. If you see this message in PROD, this means something has gone live that shouldn't've");
            var eventRegistry = _serviceProvider.GetRequiredService<DomainEventRegistry>();

            var evtGeneral = new GeneralDomainEvent("1", "x", Guid.Empty, "", new K8sNamespaceCreatedAndAwsArnConnectEventData("An event to test if everything is configured properly"));
            Log.Information($"evtGeneral payload: {evtGeneral.Payload}");
            var evtPre = new K8sNamespaceCreatedAndAwsArnConnectedEvent("onprem-43581", Guid.Empty);
            Log.Information($"evtPre payload: {evtPre.Payload}");

            // AggregateId missing due to IAggregateDomainEvents not being used.
            var evt = new DomainEventEnvelope
            {
                EventId = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                Type = eventRegistry.GetTypeNameFor(evtPre),
                Format = "application/json",
                Data = JsonConvert.SerializeObject(evtPre, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                })
            };
            
            this.AddEventToQueue(evt);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<K8sServiceDbContext>();
                var domainEventsToPublish = await dbContext
                    .DomainEvents
                    .Where(x => x.Sent == null)
                    .ToListAsync(stoppingToken);
                
                
                // Entirely for learning purposes. 
                // REMOVE BEFORE MERGING TO MASTER.
                Log.Information("Check if there's any events to publish");
                
                // New way, minimal abstractions.

                {
                    if (_eventsToBeSent.Any() == false)
                    {
                        return;
                    }
                
                    Log.Information($"Domain events to publish: {_eventsToBeSent.Count}");
                
                    var publisherFactory = scope.ServiceProvider.GetRequiredService<KafkaPublisherFactory>();
                    var eventRegistry = scope.ServiceProvider.GetRequiredService<DomainEventRegistry>();
                    
                    Log.Information("Connecting to kafka...");

                    using (var producer = publisherFactory.Create())
                    {
                        Log.Information("Connected!");

                        while (_eventsToBeSent.Any())
                        {
                            var evt = _eventsToBeSent.Dequeue();
                            var topicName = eventRegistry.GetTopicFor(evt.Type);
                            var message = MessagingHelper.CreateMessageFrom(evt);

                            var result = await producer.ProduceAsync(
                                topic: topicName,
                                key: evt.AggregateId,
                                val: message
                            );

                            if (!result.Error.HasError)
                            {
                                Log.Information($"Domain event \"{evt.Type}>{evt.EventId}\" has been published!");
                            }
                            else
                            {
                                throw new Exception($"Could not publish domain event \"{evt.Type}>{evt.EventId}\"!!!");
                            }
                        }
                    }
                }
            }
        }
    }
}