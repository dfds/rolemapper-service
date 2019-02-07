﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RolemapperService.WebApi.Models;
using RolemapperService.WebApi.Repositories.Kubernetes;
using RolemapperService.WebApi.Services;
using RolemapperService.WebApi.Validators;
using Serilog;

namespace RolemapperService.WebApi.Controllers
{
    [Route("api/namespaces")]
    [ApiController]
    public class NamespaceController : ControllerBase
    {
        private readonly IAddNamespaceRequestValidator _addNamespaceRequestValidator;
        private readonly NamespaceRespoitory _namespaceRepository;
        private readonly RoleRepository _roleRepository;
        private readonly RoleBindingRepository _roleBindingRepository;

        public NamespaceController(
            IAddNamespaceRequestValidator addNamespaceRequestValidator,
            NamespaceRespoitory namespaceRepository,
            RoleRepository roleRepository, RoleBindingRepository roleBindingRepository)
        {
            _addNamespaceRequestValidator = addNamespaceRequestValidator;
            _namespaceRepository = namespaceRepository;
            _roleRepository = roleRepository;
            _roleBindingRepository = roleBindingRepository;
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

                var roleName = await _roleRepository
                    .CreateNamespaceFullAccessRole(addNamespaceRequest.NamespaceName);

                await _roleBindingRepository.BindNamespaceRoleToGroup(
                    addNamespaceRequest.NamespaceName,
                    roleName,
                    "DFDS-ReadOnly"
                );

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