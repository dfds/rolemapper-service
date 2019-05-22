using Xunit;

namespace K8sJanitor.WebApi.IntegrationTests
{
    public class FactRunsOnK8s : FactAttribute
    {
        public FactRunsOnK8s()
        {
            if (ExecuteAgainstK8s.Allowed == false)
            {
                Skip =  "Integration tests against k8s is not allowed on this system. \n" +
                       $"Set environment variable '{ExecuteAgainstK8s.EnvironmentVariable}' to 'true' to run this test";
            }
        }
    }
}