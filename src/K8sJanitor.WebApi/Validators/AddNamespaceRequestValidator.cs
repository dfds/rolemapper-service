using System.Text.RegularExpressions;
using K8sJanitor.WebApi.Models;

namespace K8sJanitor.WebApi.Validators
{
    public class AddNamespaceRequestValidator : IAddNamespaceRequestValidator
    {
        public bool TryValidateAddNamespaceRequest(AddNamespaceRequest request, out string validationError)
        {
            validationError = string.Empty;

            if (string.IsNullOrWhiteSpace(request.NamespaceName))
            {
                validationError = "Namespace name is invalid.";
                return false;
            }

            // Namespace name 63 char max.
            if (request.NamespaceName.Length > 63)
            {
                validationError = "Namespace name is invalid. A maximum of 63 characters is allowed.";
                return false;
            }

            // Label must consist of lower case alphanumeric characters or '-', and must start and end with an alphanumeric character.
            var allowedCharactersPattern = "^[a-z0-9]([-a-z0-9]*[a-z0-9])?$";
            var match = Regex.Match(request.NamespaceName, allowedCharactersPattern);

            if (!match.Success)
            {
                validationError = "Namespace name is invalid. It must consist of lower case alphanumeric characters or '-', and must start and end with an alphanumeric character.";
                return false;
            }

            return true;
        }
    }
}