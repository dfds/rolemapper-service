using System.Collections.Generic;

namespace RolemapperService.WebApi.Services
{
    public interface IConfigMapService
    {
        string AddRoleMapping(string yaml, string roleArn, string userName, IList<string> groups);
    }
}