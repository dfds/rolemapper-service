using System;
using Xunit;

namespace K8sJanitor.WebApi.IntegrationTests
{
    public class FactRunsOnK8s : FactAttribute
    {
        private const string environmentVariable = "K8S_INTEGRATION_TESTS_ALLOWED";
        public FactRunsOnK8s()
        {
            var k8sAllowed = Environment.GetEnvironmentVariable(environmentVariable);

            if (string.IsNullOrWhiteSpace(k8sAllowed) || k8sAllowed != "true")
            {
                Skip =  "Integration tests against k8s is not allowed on this system. \n" +
                       $"Set environment variable '{environmentVariable}' to 'true' to run this test";
            }
        }
    }
}