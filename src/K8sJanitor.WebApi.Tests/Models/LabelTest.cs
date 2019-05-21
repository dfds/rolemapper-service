using System;
using Xunit;
using K8sJanitor.WebApi.Models;

namespace K8sJanitor.WebApi.Tests
{
    public class LabelTest
    {
        [Fact]
        public void Validate_WILL_reject_exclamation_mark_as_first_char()
        {
            // Arrange
            var input = "!abc";
        
            // Act / Assert
            Assert.Throws<ArgumentException>(() => Label.Create(input, input));
        }

        [Fact]
        public void Validate_WILL_reject_dollar_sign_as_first_char()
        {
            // Arrange
            var input = "abc$";
            
            // Act / Assert
            Assert.Throws<ArgumentException>(() => Label.Create(input, input));
        }

        [Fact]
        public void Validate_WILL_reject_space_in_string()
        {
            // Arrange
            var input = "a b";
            
            // Act / Assert
            Assert.Throws<ArgumentException>(() => Label.Create(input, input));
        }

        [Fact]
        public void Validate_WILL_reject_length_over_63()
        {
            // Arrange
            var input = "foobaafoobaafoobaafoobaafoobaafoobaafoobaafoobaafoobaafoobaafoob";
            
            // Act / Assert
            Assert.Throws<ArgumentException>(() => Label.Create(input, input));
        }

        [Fact]
        public void Correct_WILL_replace_spaces_with_underscores()
        {
            // Arrange
            var input = "foo baa foo baa";
        
            // Act
            var label = Label.CreateSafely(input, input);

            // Assert
            var expectedKey = "foo_baa_foo_baa";
            Assert.Equal(expectedKey, label.Key);
        }
        
        
        [Fact]
        public void Correct_WILL_truncate_string_length_over_63()
        {
            // Arrange
            var input = "foobaafoobaafoobaafoobaafoobaafoobaafoobaafoobaafoobaafoobaafoob";
            
            // Act
            var label = Label.CreateSafely(input, input);
            
            
            // // Assert
            var expectedKey = "foobaafoobaafoobaafoobaafoobaafoobaafoobaafoobaafoobaafoobaafoo";
            Assert.Equal(expectedKey, label.Key);
        }

    }
}