using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RolemapperService.WebApi.Models;
using RolemapperService.WebApi.Services;

namespace RolemapperService.WebApi.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IConfigMapService _configMapService;
        private readonly IKubernetesService _kubernetesService;

        public RoleController(IConfigMapService configMapService, IKubernetesService kubernetesService)
        {
            _configMapService = configMapService;
            _kubernetesService = kubernetesService;
        }

        [HttpGet("")]
        public ActionResult<string> Ping()
        {
            return Ok("OK");
        }

        [HttpPost("")]
        public ActionResult<string> AddRole([FromBody]AddRoleRequest addRoleRequest)
        {
            var configMapYaml = _kubernetesService.GetAwsAuthConfigMap();
            
            var updatedMapRolesYaml = _configMapService.AddRoleMapping(configMapYaml, addRoleRequest.RoleName, addRoleRequest.RoleArn);

            if (_kubernetesService.PatchAwsAuthConfigMap(updatedMapRolesYaml))
            {
                return Ok(updatedMapRolesYaml);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "An error occured trying to add the role to the config map.");
        }
    }
}
