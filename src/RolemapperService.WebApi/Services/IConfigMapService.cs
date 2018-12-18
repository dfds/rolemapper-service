using System.Collections.Generic;

namespace RolemapperService.WebApi.Services
{
    public interface IConfigMapService
    {
        string AddRoleMapping(string configMapYaml, string roleName, string roleArn);
        string AddRoleMapping(string configMapYaml, string roleArn, string userName, IList<string> groups);
        IList<string> GetReadonlyGroup();
    }
}