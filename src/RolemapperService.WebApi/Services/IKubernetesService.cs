using System.Collections.Generic;

namespace RolemapperService.WebApi.Services
{
    public interface IKubernetesService
    {
        string GetAwsAuthConfigMap();
        string GetConfigMap(string namespaceName, string configMapName);
        bool PatchAwsAuthConfigMap(string configMapYaml);
        bool PatchConfigMap(string namespaceName, string configMapName, string configMapYaml);
        bool ReplaceAwsAuthConfigMap(string configMapYaml);
        bool ReplaceConfigMap(string namespaceName, string configMapName, string configMapYaml);
    }
}