using System.Text.RegularExpressions;
using K8sJanitor.WebApi.Models;

namespace K8sJanitor.WebApi.Validators
{
    public class AddRoleRequestValidator : IAddRoleRequestValidator
    {
        public bool TryValidateAddRoleRequest(AddRoleRequest request, out string validationError)
        {
            validationError = string.Empty;

            if (string.IsNullOrWhiteSpace(request.RoleName))
            {
                validationError = "Name is invalid.";
                return false;
            }

            // Role name 64 char max.
            if (request.RoleName.Length > 63)
            {
                validationError = "Name is invalid. A maximum of 64 characters is allowed.";
                return false;
            }

            // Only alphanumeric and '+=,.@-_' allowed.
            var allowedCharactersPattern = "^[a-zA-Z0-9!@#$&()\\-`.+,/\"]*$";
            var match = Regex.Match(request.RoleName, allowedCharactersPattern, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                validationError = "Name is invalid. Only alphanumeric and '+=,.@-_' is allowed.";
                return false;
            }

            return true;
        }
    }
}