using System;
using System.Collections.Generic;
using Xunit;
using RolemapperService.WebApi.Services;
using System.Linq;

namespace RolemapperService.WebApi.Tests
{
    public class ConfigMapServiceTest
    {
        private readonly string mapRolesInput = 
@"mapRoles:
- roleARN: arn:aws:iam::228426479489:role/KubernetesAdmin
  username: kubernetes-admin:{{SessionName}}
  groups:
  - system:masters
- roleARN: arn:aws:iam::228426479489:role/KubernetesView
  username: kubernetes-view:{{SessionName}}
  groups:
  - kub-view
";

        [Fact]
        public void AddRoleMapping_GivenValidInput_ReturnsValidOutputWithGroupAdded()
        {
            // Arrange
            var sut = new ConfigMapService();
            var roleARN = "arn:aws:iam::228426479489:role/KubernetesTest";
            var username = "kubernetes-test";
            var groups = new List<string> 
            {
                "kub-test"
            };

            // Act
            var result = sut.AddRoleMapping(mapRolesInput, roleARN, username, groups);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(groups.First(), result);
        }

        [Fact]
        public void AddRoleMapping_GivenValidInput_ReturnsUsernameWithAddedSessionName()
        {
            // Arrange
            var sut = new ConfigMapService();
            var roleARN = "arn:aws:iam::228426479489:role/KubernetesTest";
            var username = "kubernetes-test";
            var groups = new List<string> 
            {
                "kub-test"
            };

            // Act
            var result = sut.AddRoleMapping(mapRolesInput, roleARN, username, groups);

            // Assert
            Assert.NotNull(result);
            Assert.Contains($"{username}:{{{{SessionName}}}}", result);
        }
    }
}
