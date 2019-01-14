using Xunit;
using RolemapperService.WebApi.Validators;
using RolemapperService.WebApi.Models;

namespace RolemapperService.WebApi.Tests
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
            var validationErros = string.Empty;

            // Act
            var validRequest = sut.TryValidateAddRoleRequest(request, out validationErros);

            // Assert
            Assert.True(validRequest);
            Assert.Equal(string.Empty, validationErros);
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
            var validationErros = string.Empty;

            // Act
            var validRequest = sut.TryValidateAddRoleRequest(request, out validationErros);

            // Assert
            Assert.False(validRequest);
            Assert.NotEqual(string.Empty, validationErros);
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
            var validationErros = string.Empty;

            // Act
            var validRequest = sut.TryValidateAddRoleRequest(request, out validationErros);

            // Assert
            Assert.False(validRequest);
            Assert.NotEqual(string.Empty, validationErros);
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
            var validationErros = string.Empty;

            // Act
            var validRequest = sut.TryValidateAddRoleRequest(request, out validationErros);

            // Assert
            Assert.False(validRequest);
            Assert.NotEqual(string.Empty, validationErros);
        }
    }
}
