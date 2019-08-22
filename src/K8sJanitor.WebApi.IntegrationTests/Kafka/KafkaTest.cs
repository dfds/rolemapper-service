using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace K8sJanitor.WebApi.IntegrationTests.Kafka
{
    // If a test seems to be stuck, ensure that all manual steps in ManualEvents.cs has been taken.
    public class KafkaTest
    {

        [Fact]
        public async Task ProduceEventAndCheckExternallyThatItHasBeenReceived()
        {
            var services = Helper.SetupV2();
            var conf = Helper.GetConfiguration();
            await Helper.ResetFakeServer(services);

            var producer = Helper.CreateKafkaProducer();
            var consumer = Helper.CreateKafkaConsumer();
            consumer.Subscribe(conf.Test.Topic);
            var payload = Helper.GetRawPayload("k8s_namespace_created_and_aws_arn_connect.json");
            
            var message = new Message<string, string>
            {
                Key = "1st",
                Value = payload
            };
            
            var apiCallsReceived = await Helper.CallFakeServer("/api-calls-received", services);
            Assert.Equal(1, apiCallsReceived.ApiCallsReceived);
            
            var consumerTask = Task.Run(() =>
            {
                var msg = consumer.Consume();
                consumer.Close();
                return msg;
            });
            
            // Wait for Consume() to initialise prior services
            Thread.Sleep(5000);
            
            var result = await producer.ProduceAsync(conf.Test.Topic, message);
            
            Assert.Equal(
                expected: payload, 
                actual: consumerTask.Result.Value);

            var res = Helper.CallFakeServer("/api-calls-received", services).Result;
            Assert.Equal(res.ApiCallsReceived, apiCallsReceived.ApiCallsReceived + 1);
            Assert.Equal(1, res.KafkaMessageReceived);

            await Helper.CallFakeServer("/api-calls-reset", services);
        }

        [Fact]
        public async Task ProduceExistingEvents()
        {
            var services = Helper.SetupV2();
            var conf = Helper.GetConfiguration();
            var producer = Helper.CreateKafkaProducer();
            var payloads = Helper.GetRawPayloads();
            
            await Helper.ResetFakeServer(services);
            
            var apiCallsReceived = await Helper.CallFakeServer("/api-calls-received", services);
            Assert.Equal(1, apiCallsReceived.ApiCallsReceived);
            Assert.Equal(0, apiCallsReceived.KafkaMessageReceived);

            foreach (var rawEvent in payloads)
            {
                var message = new Message<string, string>
                {
                    Value = rawEvent
                };

                await producer.ProduceAsync(conf.Test.Topic, message);
            }
            
            // Wait for "FakeServer" to consume all events.
            Thread.Sleep(3000);
            
            var res = await Helper.CallFakeServer("/api-calls-received", services);
            Assert.Equal(res.ApiCallsReceived, apiCallsReceived.ApiCallsReceived + 1);
            Assert.Equal(payloads.Count(), res.KafkaMessageReceived);
            await Helper.CallFakeServer("/api-calls-reset", services);
        }

        [Fact]
        public async Task ConsumeExistingEvents()
        {
            var services = Helper.SetupV2();
            var conf = Helper.GetConfiguration();
            var producer = Helper.CreateKafkaProducer();
            var consumer = Helper.CreateKafkaConsumer();
            var payloads = Helper.GetRawPayloads();
            
            consumer.Subscribe(conf.Test.Topic);
            await Helper.ResetFakeServer(services);
            
            var apiCallsReceived = await Helper.CallFakeServer("/api-calls-received", services);
            Assert.Equal(1, apiCallsReceived.ApiCallsReceived);
            Assert.Equal(0, apiCallsReceived.KafkaMessageReceived);

            var expectedAmountOfEventsToConsume = payloads.Count();
            var consumerTask = Task.Run(() =>
            {
                var messages = new List<ConsumeResult<string, string>>();

                while (expectedAmountOfEventsToConsume != 0)
                {
                    var msg = consumer.Consume();
                    messages.Add(msg);
                    expectedAmountOfEventsToConsume -= 1;
                }
                
                consumer.Close();
                return messages;
            });
            
            // Wait for Consume() to initialise prior services
            Thread.Sleep(3000);
            
            foreach (var rawEvent in payloads)
            {
                var message = new Message<string, string>
                {
                    Value = rawEvent
                };

                await producer.ProduceAsync(conf.Test.Topic, message);
            }
            
            // Wait for "FakeServer" to consume all events.
            Thread.Sleep(3000);

            var res = consumerTask.Result;
            Assert.Equal(payloads.Count(), res.Count);
            var resExternal = Helper.CallFakeServer("/api-calls-received", services).Result;
            Assert.Equal(resExternal.ApiCallsReceived, apiCallsReceived.ApiCallsReceived + 1);
            Assert.Equal(payloads.Count(), resExternal.KafkaMessageReceived);
        }

        [Fact]
        public async Task QueryRestApiConsumeEvent()
        {
            var serviceProvider = Helper.SetupServiceProviderWithConsumerAndProducer(useManualEvents: true);
            var services = Helper.SetupV2();
            await Helper.ResetFakeServer(services);
            var eventRegistry = serviceProvider.GetRequiredService<DomainEventRegistry>();
            
            const string topic = "build.capabilities";
            

            using (var scope = serviceProvider.CreateScope())
            {
                ManualEvents.RegisterEvents(eventRegistry, topic, scope);
            }
            
            var consumer = Helper.SetupKafkaConsumption(serviceProvider.CreateScope());

            var consumerTask = Task.Run(() =>
            {
                var msg = consumer.Consume();
                consumer.Close();
                return msg;
            });
            
            // Wait for Consume() to initialise prior services
            Thread.Sleep(3000);

            var payload = new K8sNamespaceCreatedAndAwsArnConnectedEvent("kafkaTest", Guid.Parse("f8bbe9e1-cdda-41fb-9781-bf43dbc18a47"), Guid.Parse("2a70d5ac-5e1f-4e1d-8d81-4c4cbda7b9d9"));
            var payloadAsJsonString = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var resp = await Helper.PostFakeServer("/api-create-event", serviceProvider.CreateScope(), new StringContent(payloadAsJsonString, Encoding.UTF8, "application/json"));

            var consumeResult = consumerTask.Result;

            var res = await Helper.CallFakeServer("/api-calls-received", services);
            
            await Helper.ResetFakeServer(services);

            Assert.Equal(
                expected: "{\"version\":\"1\",\"eventName\":\"k8s_namespace_created_and_aws_arn_connected\",\"x-correlationId\":\"\",\"x-sender\":\"K8sJanitor.WebApi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"payload\":{\"namespaceName\":\"kafkaTest\",\"contextId\":\"f8bbe9e1-cdda-41fb-9781-bf43dbc18a47\",\"capabilityId\":\"2a70d5ac-5e1f-4e1d-8d81-4c4cbda7b9d9\"}}", 
                actual: consumeResult.Value);
            // k8s_namespace_created_and_aws_arn_connect.json
            
            Assert.Equal(1, res.KafkaMessageReceived);
        }
    }
}