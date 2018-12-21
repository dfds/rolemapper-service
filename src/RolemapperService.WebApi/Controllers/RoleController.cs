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
        private readonly IKubernetesService _kubernetesService;

        public RoleController(IKubernetesService kubernetesService)
        {
            _kubernetesService = kubernetesService;
        }

        [HttpGet("")]
        public async Task<ActionResult<string>> GetRoleMap()
        {
            var configMapRoleMap = await _kubernetesService.GetAwsAuthConfigMapRoleMap();
            return Ok(configMapRoleMap);
        }

        [HttpPost("")]
        public async Task<ActionResult<string>> AddRole([FromBody]AddRoleRequest addRoleRequest)
        {
            var updatedMapRolesYaml = await _kubernetesService.ReplaceAwsAuthConfigMapRoleMap(addRoleRequest.RoleName, addRoleRequest.RoleArn);
            
            return Ok(updatedMapRolesYaml);
        }
    }
}
