using System;
using RolemapperService.WebApi.Models;
using RolemapperService.WebApi.Validators;

namespace RolemapperService.WebApi.Tests.TestDoubles
{
    public class AddRoleRequestValidatorStub : IAddRoleRequestValidator
    {
        public bool TryValidateAddRoleRequest(AddRoleRequest request, out string validationError)
        {
            validationError = String.Empty;
            return true;
        }
    }
}