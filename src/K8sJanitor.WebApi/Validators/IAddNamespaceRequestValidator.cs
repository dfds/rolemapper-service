using K8sJanitor.WebApi.Models;

namespace K8sJanitor.WebApi.Validators
{
    public interface IAddNamespaceRequestValidator
    {
        bool TryValidateAddNamespaceRequest(AddNamespaceRequest request, out string validationError);
    }
}