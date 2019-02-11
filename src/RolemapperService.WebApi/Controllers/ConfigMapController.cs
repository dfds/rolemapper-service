using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RolemapperService.WebApi.Extensions;
using RolemapperService.WebApi.Repositories;
using RolemapperService.WebApi.Services;
using Serilog;

namespace RolemapperService.WebApi.Controllers
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