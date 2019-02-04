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
    [Route("api/namespaces")]
    [ApiController]
    public class NamespaceController : ControllerBase
    {
        private readonly IKubernetesService _kubernetesService;

        public NamespaceController(IKubernetesService kubernetesService)
        {
            _kubernetesService = kubernetesService;
        }

        [HttpPost("")]
        public async Task<ActionResult> AddNamespace([FromBody]AddNamespaceRequest addNamespaceRequest)
        {
            try
            {
                await _kubernetesService.CreateNamespace(addNamespaceRequest.NamespaceName);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"An error occured trying to create the namespace: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occured trying to create the namespace: {ex.Message}");
            }
        }
    }
}
