using System;
using System.Text.RegularExpressions;

namespace K8sJanitor.WebApi.Models
{
    public class Label
    {
        private const int MAX_LENGTH = 63;
        public string Key { get; }
        public string Value { get; }

        /// <summary>
        /// Alters key and value parameters to create valid inputs
        /// </summary>
        public static Label CreateSafely(string key, string value)
        {
            return Create(Correct(key), Correct(value));
        }

        public static Label Create(string key, string value)
        {
            return new Label(key, value);
        }

        private Label(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("May not be empty", nameof(key));
            }

            Validate(nameof(key), key);

            Validate(nameof(value), value);

            Key = key;
            Value = value;
        }

        private static string Correct(string value)
        {
            value = value.Replace(" ", "_");
            value = value.Substring(0, Math.Min(value.Length, MAX_LENGTH));

            return value;
        }

        private void Validate(string parameterName, string parameterValue)
        {
            if (Regex.IsMatch(parameterValue, @"^[a-z0-9A-Z]") == false)
            {
                throw new ArgumentException("Must start with an alphanumeric character", parameterName);
            }

            if (Regex.IsMatch(parameterValue, @"[a-z0-9A-Z]$") == false)
            {
                throw new ArgumentException("Must end with an alphanumeric character", parameterName);
            }

            if (Regex.IsMatch(parameterValue, @"^[a-z0-9A-Z-_.]*$") == false)
            {
                throw new ArgumentException("Can only contain alphanumeric characters,'.','-' and '_'", parameterName);
            }

            if (MAX_LENGTH < parameterValue.Length)
            {
                throw new ArgumentException($"max length is {MAX_LENGTH} current length is {parameterValue.Length}",
                    parameterName);
            }
        }
    }
}