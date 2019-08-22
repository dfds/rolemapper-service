using System;
using K8sJanitor.WebApi.IntegrationTests.Kafka;
using Xunit;

namespace K8sJanitor.WebApi.IntegrationTests
{
    public class FactRunsWithKafka : FactAttribute
    {
        public FactRunsWithKafka()
        {
            var allowed = Environment.GetEnvironmentVariable("EXECUTE_AGAINST_KAFKA") != null 
                ? Boolean.Parse(Environment.GetEnvironmentVariable("EXECUTE_AGAINST_KAFKA") ?? throw new NullReferenceException("Unable to convert ENV VAR to bool")) 
                : false;

            if (!allowed)
            {
                Skip = "Integration tests against kafka is not enabled on this system. \n" +
                       "Set environment variable EXECUTE_AGAINST_KAFKA to 'true' to run Kafka integration tests.";
            }
        }
    }
}