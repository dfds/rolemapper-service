using K8sJanitor.WebApi.Models;
using Xunit;

namespace K8sJanitor.WebApi.Tests
{
    public class StringSubstitutableTest
    {
        public class StringSubstitutableA : StringSubstitutable
        {
            public StringSubstitutableA(string name) : base(name)
            {
            }
        }
        
        public class StringSubstitutableB: StringSubstitutable
        {
            public StringSubstitutableB(string name) : base(name)
            {
            }
        }

        
        [Fact]
        public void Will_Equal_string()
        {
            // Arrange
            var sub = new StringSubstitutableA("foo");
            
            
            // Act
            // Assert
            Assert.Equal("foo",sub);
        }

        [Fact]
        public void Different_StringSubstitutables_Will_Not_Equal()
        {
            // Arrange
            var typeA = new StringSubstitutableA("foo");
            var typeB = new StringSubstitutableB("foo");
            
            
            // Act
            var equals = typeA.Equals(typeB);
            
            
            // Assert
            Assert.False(equals);
        }

        
        [Fact]
        public void StringSubstitutables_Will_Equal_Its_Own_Type()
        {
            // Arrange
            var obj1 = new StringSubstitutableA("foo");
            var obj2 = new StringSubstitutableA("foo");
            
            
            // Act
            // Assert
            Assert.Equal(obj1, obj2);
        }


        [Fact]
        public void StringSubstitutables_Gets_HashCode_From_value()
        {
            // Arrange
            var str = "foo baa";
            var stringSubstitutable = new StringSubstitutableA("foo baa");
            
            // Act
            // Assert
            Assert.Equal(str.GetHashCode(),stringSubstitutable.GetHashCode());
        }
    }
}