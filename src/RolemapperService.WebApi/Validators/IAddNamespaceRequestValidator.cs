using RolemapperService.WebApi.Models;

namespace RolemapperService.WebApi.Validators
{
    public interface IAddNamespaceRequestValidator
    {
        bool TryValidateAddNamespaceRequest(AddNamespaceRequest request, out string validationError);
    }
}