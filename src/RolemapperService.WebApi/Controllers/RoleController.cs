using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RolemapperService.WebApi.Models;
using RolemapperService.WebApi.Services;
using RolemapperService.WebApi.Validators;
using Serilog;

namespace RolemapperService.WebApi.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IKubernetesService _kubernetesService;
        private readonly IAddRoleRequestValidator _addRoleRequestValidator;

        public RoleController(IKubernetesService kubernetesService, IAddRoleRequestValidator addRoleRequestValidator)
        {
            _kubernetesService = kubernetesService;
            _addRoleRequestValidator = addRoleRequestValidator;
        }

        [HttpPost("")]
        public async Task<ActionResult<string>> AddRole([FromBody]AddRoleRequest addRoleRequest)
        {
            if (!_addRoleRequestValidator.TryValidateAddRoleRequest(addRoleRequest, out string validationError))
            {
                Log.Warning($"Create role called with invalid input. Validation error: {validationError}");
                return BadRequest(validationError);
            }

            var updatedMapRolesYaml = string.Empty;

            try
            {
                updatedMapRolesYaml = await _kubernetesService.ReplaceAwsAuthConfigMapRoleMap(addRoleRequest.RoleName, addRoleRequest.RoleArn);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occured trying to create the role mapping: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occured trying to create the role mapping: {ex.Message}");
            }

            return Ok(updatedMapRolesYaml);
        }

        [HttpGet("/api/configmap")]
        public async Task<ActionResult<string>> GetConfigMap()
        {
            var configMapRoleMap = string.Empty;

            try
            {
                configMapRoleMap = await _kubernetesService.GetAwsAuthConfigMap();
            }
            catch (Exception ex)
            {
                Log.Error($"An error occured trying to get the config map: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occured trying to get the config map: {ex.Message}");
            }

            return Ok(configMapRoleMap);
        }
    }
}
