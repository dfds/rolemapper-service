using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public RoleController(IConfigMapService configMapService)
        {
            _configMapService = configMapService;
        }

        [HttpGet("")]
        public ActionResult<string> Ping()
        {
            return Ok("OK");
        }

        [HttpPost("")]
        public ActionResult<string> AddRole([FromBody]AddRoleRequest addRoleRequest)
        {
            // TODO: Get from Kubernetes API.
            var mapRolesYaml = _mapRolesInput;
            
            var updatedMapRolesYaml = _configMapService.AddRoleMapping(mapRolesYaml, addRoleRequest.RoleArn);

            // TODO: Call Kubernetes API to update config map.

            return Ok(updatedMapRolesYaml);
        }

        private readonly string _mapRolesInput = 
@"mapRoles:
- roleARN: arn:aws:iam::228426479489:role/KubernetesAdmin
  username: kubernetes-admin:{{SessionName}}
  groups:
  - system:masters
- roleARN: arn:aws:iam::228426479489:role/KubernetesView
  username: kubernetes-view:{{SessionName}}
  groups:
  - kub-view
";
    }
}
