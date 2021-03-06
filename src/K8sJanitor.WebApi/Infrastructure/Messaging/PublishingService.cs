using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public class PublishingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IPublishingEventsQueue _eventsQueue;

        public PublishingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _eventsQueue = _serviceProvider.GetService<IPublishingEventsQueue>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Starting event publisher.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DoWork(stoppingToken);
                }
                catch (Exception err)
                {
                    Log.Error(err, "Error processing and/or publishing domain events to message broker.");
                }

                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }
        
        private async Task DoWork(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                {
                    if (_eventsQueue.AnyEventInQueue() == false)
                    {
                        return;
                    }
                
                    Log.Information($"Domain events to publish: {_eventsQueue.QueueCount()}");
                
                    var publisherFactory = scope.ServiceProvider.GetRequiredService<KafkaPublisherFactory>();
                    var eventRegistry = scope.ServiceProvider.GetRequiredService<IDomainEventRegistry>();
                    
                    Log.Information("Connecting to kafka...");

                    using (var producer = publisherFactory.Create())
                    {
                        Log.Information("Connected!");

                        while (_eventsQueue.AnyEventInQueue())
                        {
                            var evt = _eventsQueue.Dequeue();
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
                                Log.Information($"Domain event \"{evt.Type}>{evt.EventId}\" has been published!");

                            }
                            catch (Exception)
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