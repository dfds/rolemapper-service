using System;
using System.Text.RegularExpressions;

namespace K8sJanitor.WebApi.Models
{
    public class NamespaceName : StringSubstitutable
    {
        private NamespaceName(string name) : base(name)
        {
            
        }

        public static NamespaceName Create(string name)
        {
            name = name.ToLower().Replace(" ", "-");
            if (Regex.IsMatch(name, @"[a-z0-9\-\.]") == false)
            {
                throw new ArgumentException($"namespace name can only contain letters, digits, -, and .");
            }

            const int maxLength = 253;
            if (maxLength < name.Length)
            {
                throw new ArgumentException("Namespace name can maximum be 253 characters");
            }
            
            
            return new NamespaceName(name);
        }
    }
}