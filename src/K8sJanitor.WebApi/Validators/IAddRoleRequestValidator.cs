using K8sJanitor.WebApi.Models;

namespace K8sJanitor.WebApi.Validators
{
    public interface IAddRoleRequestValidator
    {
        bool TryValidateAddRoleRequest(AddRoleRequest request, out string validationError);
    }
}