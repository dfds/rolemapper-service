using System.Collections.Generic;
using System.Threading.Tasks;
using RolemapperService.WebApi.Services;

namespace RolemapperService.WebApi.Tests.TestDoubles
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