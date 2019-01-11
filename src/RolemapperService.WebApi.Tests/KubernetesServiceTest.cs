using System;
using System.Collections.Generic;
using Xunit;
using RolemapperService.WebApi.Services;
using System.Linq;
using RolemapperService.WebApi.Tests.TestDoubles;

namespace RolemapperService.WebApi.Tests
{
    public class KubernetesServiceTest
    {
        [Fact]
        public async void ReplaceAwsAuthConfigMapRoleMap_GivenValidInput_ReturnsValidOutput()
        {
            // Arrange
            var sut = new KubernetesService(new KubernetesRepositoryStub(), new ConfigMapService());
            var roleName = "KubernetesTest";
            var roleARN = "arn:aws:iam::228426479489:role/KubernetesTest";

            // Act
            var result = await sut.ReplaceAwsAuthConfigMapRoleMap(roleName, roleARN);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(roleName, result);
        }

        [Fact]
        public async void PatchAwsAuthConfigMapRoleMap_GivenValidInput_ReturnsValidOutput()
        {
            // Arrange
            var sut = new KubernetesService(new KubernetesRepositoryStub(), new ConfigMapService());
            var roleName = "KubernetesTest";
            var roleARN = "arn:aws:iam::228426479489:role/KubernetesTest";

            // Act
            var result = await sut.PatchAwsAuthConfigMapRoleMap(roleName, roleARN);
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains(roleName, result);
        }

        [Fact]
        public async void GetAwsAuthConfigMap_ReturnsValidOutput()
        {
            // Arrange
            var sut = new KubernetesService(new KubernetesRepositoryStub(), new ConfigMapService());
            
            // Act
            var result = await sut.GetAwsAuthConfigMap();

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Kind: ConfigMap", result);
        }
    }
}
