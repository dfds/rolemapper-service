using K8sJanitor.WebApi.Models;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Validators;
using Microsoft.AspNetCore.Mvc;
using Moq.AutoMock;
using System.Threading.Tasks;
using Xunit;

namespace K8sJanitor.WebApi.Tests.Controllers.NamespaceController
{
    public class AddNamespaceFacts
    {
        [Fact]
        public async Task Will_Handle_NamespaceRequest()
        {
            //Arrange
            var mocker = new AutoMocker();
            var validationResult = string.Empty;
            var addNamespaceRequest = mocker.CreateInstance<AddNamespaceRequest>();
            var namespaceValidatorMock = mocker.GetMock<IAddNamespaceRequestValidator>();
            var namespaceRepositoryMock = mocker.GetMock<INamespaceRepository>();
            var roleRepository = mocker.GetMock<IRoleRepository>();
            var sut = mocker.CreateInstance<WebApi.Controllers.NamespaceController>();

            namespaceValidatorMock.Setup(o => o.TryValidateAddNamespaceRequest(addNamespaceRequest, out validationResult)).Returns(true);

            //Act
            var result = await sut.AddNamespace(addNamespaceRequest);

            //Assert           
            Assert.IsType<OkResult>(result);
        }
    }
}