using System;
using K8sJanitor.WebApi.Models;
using K8sJanitor.WebApi.Validators;

namespace K8sJanitor.WebApi.Tests.TestDoubles
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