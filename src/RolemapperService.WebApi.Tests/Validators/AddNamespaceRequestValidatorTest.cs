using Xunit;
using RolemapperService.WebApi.Validators;
using RolemapperService.WebApi.Models;

namespace RolemapperService.WebApi.Tests
{
    public class AddNamespaceRequestValidatorTest
    {
        [Fact]
        public void TryValidateAddNamespaceRequest_GivenValidInput_Validates()
        {
            // Arrange
            var sut = new AddNamespaceRequestValidator();
            var request = new AddNamespaceRequest
            {
                NamespaceName = "my-namespace"
            };
            var validationErrors = string.Empty;

            // Act
            var validRequest = sut.TryValidateAddNamespaceRequest(request, out validationErrors);

            // Assert
            Assert.True(validRequest);
            Assert.Equal(string.Empty, validationErrors);
        }

        [Fact]
        public void TryValidateAddNamespaceRequest_GivenInvalidCharacter_DoesNotValidate()
        {
            // Arrange
            var sut = new AddNamespaceRequestValidator();
            var request = new AddNamespaceRequest
            {
                NamespaceName = "å"
            };
            var validationErrors = string.Empty;

            // Act
            var validRequest = sut.TryValidateAddNamespaceRequest(request, out validationErrors);

            // Assert
            Assert.False(validRequest);
            Assert.NotEqual(string.Empty, validationErrors);
        }

        [Fact]
        public void TryValidateAddNamespaceRequest_GivenTooLongNamespaceName_DoesNotValidate()
        {
            // Arrange
            var sut = new AddNamespaceRequestValidator();
            var namespaceNameLongerThan64Characters = new string('a', 64);
            var request = new AddNamespaceRequest
            {
                NamespaceName = namespaceNameLongerThan64Characters
            };
            var validationErrors = string.Empty;

            // Act
            var validRequest = sut.TryValidateAddNamespaceRequest(request, out validationErrors);

            // Assert
            Assert.False(validRequest);
            Assert.NotEqual(string.Empty, validationErrors);
        }

        [Fact]
        public void TryValidateAddNamespaceRequest_GivenEmptyNamespaceName_DoesNotValidate()
        {
            // Arrange
            var sut = new AddNamespaceRequestValidator();
            var emptyNamespaceName = string.Empty;
            var request = new AddNamespaceRequest
            {
                NamespaceName = emptyNamespaceName
            };
            var validationErrors = string.Empty;

            // Act
            var validRequest = sut.TryValidateAddNamespaceRequest(request, out validationErrors);

            // Assert
            Assert.False(validRequest);
            Assert.NotEqual(string.Empty, validationErrors);
        }

        [Fact]
        public void TryValidateAddNamespaceRequest_GivenUpperCaseNamespaceName_DoesNotValidate()
        {
            // Arrange
            var sut = new AddNamespaceRequestValidator();
            var request = new AddNamespaceRequest
            {
                NamespaceName = "My-Namespace"
            };
            var validationErrors = string.Empty;

            // Act
            var validRequest = sut.TryValidateAddNamespaceRequest(request, out validationErrors);

            // Assert
            Assert.False(validRequest);
            Assert.NotEqual(string.Empty, validationErrors);
        }
    }
}
