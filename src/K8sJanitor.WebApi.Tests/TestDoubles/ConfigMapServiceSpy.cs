using System.Collections.Generic;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Services;

namespace K8sJanitor.WebApi.Tests.TestDoubles
{
    public class ConfigMapServiceSpy : IConfigMapService
    {
        public List<KeyValuePair<string,string>> Roles { get;}

        public ConfigMapServiceSpy()
        {
            Roles = new List<KeyValuePair<string, string>>();
        }
        
        public Task AddRole(string roleName, string roleArn)
        {
            Roles.Add(new KeyValuePair<string, string>(roleName,roleArn));

            return Task.CompletedTask;
        }
    }
}