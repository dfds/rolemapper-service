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
        @"- roleARN: arn:aws:iam::228426479489:role/KubernetesAdmin
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

        [Fact]
        public void AddRoleMapping_MultipleMappings_ReturnsMultipleUsernamesAdded()
        {
            // Arrange
            var sut = new ConfigMapService();
            var roleARN1 = "arn:aws:iam::228426479489:role/KubernetesTest";
            var username1 = "kubernetes-test";
            var roleARN2 = "arn:aws:iam::228426479489:role/KubernetesTest2";
            var username2 = "kubernetes-test2";
            var groups = new List<string> 
            {
                "kub-test"
            };

            // Act
            var result1 = sut.AddRoleMapping(mapRolesInput, roleARN1, username1, groups);
            var result2 = sut.AddRoleMapping(result1, roleARN2, username2, groups);
            
            // Assert
            Assert.NotNull(result2);
            Assert.Contains(username1, result2);
            Assert.Contains(username2, result2);
        }

        [Fact]
        public void AddRoleMapping_MultipleMappings_DoesntAddYamlDocumentEnd()
        {
            // Arrange
            string YamlDocumentEnd = "...";
            var sut = new ConfigMapService();
            var roleARN1 = "arn:aws:iam::228426479489:role/KubernetesTest";
            var username1 = "kubernetes-test";
            var roleARN2 = "arn:aws:iam::228426479489:role/KubernetesTest2";
            var username2 = "kubernetes-test2";
            var groups = new List<string> 
            {
                "kub-test"
            };

            // Act
            var result1 = sut.AddRoleMapping(mapRolesInput, roleARN1, username1, groups);
            var result2 = sut.AddRoleMapping(result1, roleARN2, username2, groups);
            
            // Assert
            Assert.NotNull(result2);
            Assert.DoesNotContain(YamlDocumentEnd, result2);
        }

        [Fact]
        public void AddReadOnlyRoleMapping_GivenValidInput_ReturnsValidOutputWithReadOnlyGroupAdded()
        {
            // Arrange
            var sut = new ConfigMapService();
            var roleARN = "arn:aws:iam::228426479489:role/KubernetesTest";
            var username = "kubernetes-test";
            var readOnlyGroup = "DFDS-ReadOnly";

            // Act
            var result = sut.AddReadOnlyRoleMapping(mapRolesInput, roleARN, username);
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains(readOnlyGroup, result);
        }
    }
}
