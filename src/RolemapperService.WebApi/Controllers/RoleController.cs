using System;
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
        private readonly IConfigMapPersistanceService _configMapPersistanceService;

        public RoleController(IKubernetesService kubernetesService, 
                              IAddRoleRequestValidator addRoleRequestValidator,
                              IConfigMapPersistanceService configMapPersistanceService)
        {
            _kubernetesService = kubernetesService;
            _addRoleRequestValidator = addRoleRequestValidator;
            _configMapPersistanceService = configMapPersistanceService;
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
                await _configMapPersistanceService.StoreConfigMap(updatedMapRolesYaml);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occured trying to create the role mapping: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occured trying to create the role mapping: {ex.Message}");
            }

            return Ok(updatedMapRolesYaml);
        }
    }
}
