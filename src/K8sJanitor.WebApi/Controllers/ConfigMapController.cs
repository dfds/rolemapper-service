using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using K8sJanitor.WebApi.Extensions;
using K8sJanitor.WebApi.Repositories;
using K8sJanitor.WebApi.Services;
using Serilog;

namespace K8sJanitor.WebApi.Controllers
{
    [Route("/api/configmap")]
    [ApiController]
    public class ConfigMapController : ControllerBase
    {
        private readonly IAwsAuthConfigMapRepository _awsAuthConfigMapRepository;

        public ConfigMapController(IAwsAuthConfigMapRepository awsAuthConfigMapRepository)
        {
            _awsAuthConfigMapRepository = awsAuthConfigMapRepository;
        }

        [HttpGet("")]
        public async Task<ActionResult<string>> GetConfigMap()
        {
            try
            {
                var configMapRoleMap = await _awsAuthConfigMapRepository.GetConfigMap();
                return Ok(configMapRoleMap.SerializeToYaml());
            }
            catch (Exception ex)
            {
                Log.Error($"An error occured trying to get the config map: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occured trying to get the config map: {ex.Message}");
            }
        }
    }
}