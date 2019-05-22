using System;

namespace K8sJanitor.WebApi
{
    public static class ExecuteAgainstK8s
    {
        public const string EnvironmentVariable = "EXECUTE_AGAINST_K8S";

        public static bool Allowed
        {
            get
            {
                var k8sAllowed = Environment.GetEnvironmentVariable(EnvironmentVariable);

                return !string.IsNullOrWhiteSpace(k8sAllowed) && k8sAllowed == "true";
            }
        }
    }
}