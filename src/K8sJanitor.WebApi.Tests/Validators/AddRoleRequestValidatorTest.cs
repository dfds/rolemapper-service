using Xunit;
using K8sJanitor.WebApi.Validators;
using K8sJanitor.WebApi.Models;

namespace K8sJanitor.WebApi.Tests
{
    public class AddRoleRequestValidatorTest
    {
        [Fact]
        public void TryValidateAddRoleRequest_GivenValidInput_Validates()
        {
            // Arrange
            var sut = new AddRoleRequestValidator();
            var request = new AddRoleRequest
            {
                RoleName = "MyRole"
            };
            var validationErrors = string.Empty;

            // Act
            var validRequest = sut.TryValidateAddRoleRequest(request, out validationErrors);

            // Assert
            Assert.True(validRequest);
            Assert.Equal(string.Empty, validationErrors);
        }

        [Fact]
        public void TryValidateAddRoleRequest_GivenInvalidCharacter_DoesNotValidate()
        {
            // Arrange
            var sut = new AddRoleRequestValidator();
            var request = new AddRoleRequest
            {
                RoleName = "å"
            };
            var validationErrors = string.Empty;

            // Act
            var validRequest = sut.TryValidateAddRoleRequest(request, out validationErrors);

            // Assert
            Assert.False(validRequest);
            Assert.NotEqual(string.Empty, validationErrors);
        }

        [Fact]
        public void TryValidateAddRoleRequest_GivenTooLongRoleName_DoesNotValidate()
        {
            // Arrange
            var sut = new AddRoleRequestValidator();
            var roleNameLongerThan64Characters = new string('*', 65);
            var request = new AddRoleRequest
            {
                RoleName = roleNameLongerThan64Characters
            };
            var validationErrors = string.Empty;

            // Act
            var validRequest = sut.TryValidateAddRoleRequest(request, out validationErrors);

            // Assert
            Assert.False(validRequest);
            Assert.NotEqual(string.Empty, validationErrors);
        }

        [Fact]
        public void TryValidateAddRoleRequest_GivenEmptyRoleName_DoesNotValidate()
        {
            // Arrange
            var sut = new AddRoleRequestValidator();
            var emptyRoleName = string.Empty;
            var request = new AddRoleRequest
            {
                RoleName = emptyRoleName
            };
            var validationErrors = string.Empty;

            // Act
            var validRequest = sut.TryValidateAddRoleRequest(request, out validationErrors);

            // Assert
            Assert.False(validRequest);
            Assert.NotEqual(string.Empty, validationErrors);
        }
    }
}
