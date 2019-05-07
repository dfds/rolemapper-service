using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using K8sJanitor.WebApi.Models;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Validators;
using Serilog;

namespace K8sJanitor.WebApi.Controllers
{
    [Route("api/namespaces")]
    [ApiController]
    public class NamespaceController : ControllerBase
    {
        private readonly IAddNamespaceRequestValidator _addNamespaceRequestValidator;
        private readonly INamespaceRepository _namespaceRepository;
        private readonly IRoleRepository _roleRepository;

        public NamespaceController(
            IAddNamespaceRequestValidator addNamespaceRequestValidator,
            INamespaceRepository namespaceRepository,
            IRoleRepository roleRepository 
        )
        {
            
            _addNamespaceRequestValidator = addNamespaceRequestValidator;
            _namespaceRepository = namespaceRepository;
            _roleRepository = roleRepository;
        }
        
        [HttpPost("")]
        public async Task<ActionResult> AddNamespace([FromBody] AddNamespaceRequest addNamespaceRequest)
        {
            if (!_addNamespaceRequestValidator.TryValidateAddNamespaceRequest(addNamespaceRequest,
                out string validationError))
            {
                Log.Warning($"Add namespace called with invalid input. Validation error: {validationError}");
                return BadRequest(validationError);
            }

            try
            {
                await _namespaceRepository.CreateNamespace(
                    addNamespaceRequest.NamespaceName,
                    addNamespaceRequest.RoleName
                );

               await _roleRepository
                    .CreateNamespaceFullAccessRole(addNamespaceRequest.NamespaceName);

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"An error occured trying to create the namespace: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occured trying to create the namespace: {ex.Message}");
            }
        }
    }
}