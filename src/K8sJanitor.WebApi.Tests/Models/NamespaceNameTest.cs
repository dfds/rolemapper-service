using System;
using K8sJanitor.WebApi.Models;
using Xunit;

namespace K8sJanitor.WebApi.Tests
{
    public class NamespaceNameTest
    {
        [Fact]
        public void create_will_throw_error_if_comma_is_in_name()
        {
            Assert.Throws<ArgumentException>(() => NamespaceName.Create(","));
        }

        [Fact]
        public void create_will_lowercase_characters()
        {
            // Arrange
            var name = "foo";
            
            
            // Act
            var namespaceName = NamespaceName.Create(name.ToUpper());

            
            // Assert
            Assert.Equal(name, namespaceName);
        }

        [Fact]
        public void create_will_throw_error_if_name_length_is_254()
        {
            // Arrange
            var name =
                "foobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoobafoob";
            
            // Act // Assert
            Assert.Throws<ArgumentException>(() => NamespaceName.Create(name));
 
        }
    }
}