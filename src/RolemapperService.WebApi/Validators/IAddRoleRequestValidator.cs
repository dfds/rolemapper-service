using RolemapperService.WebApi.Models;

namespace RolemapperService.WebApi.Validators
{
    public interface IAddRoleRequestValidator
    {
        bool TryValidateAddRoleRequest(AddRoleRequest request, out string validationError);
    }
}