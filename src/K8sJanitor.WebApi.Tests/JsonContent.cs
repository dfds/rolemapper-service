using Xunit;
using K8sJanitor.WebApi.Validators;
using K8sJanitor.WebApi.Models;
using System.Net.Http;
using System.Text;

namespace K8sJanitor.WebApi.Tests
{
    public class JsonContent : StringContent
    {
        public JsonContent(string content) 
            : base(content, Encoding.UTF8, "application/json")
        {
            
        }
    }
}
