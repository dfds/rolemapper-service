using Xunit;
using RolemapperService.WebApi.Validators;
using RolemapperService.WebApi.Models;
using System.Net.Http;
using System.Text;

namespace RolemapperService.WebApi.Tests
{
    public class JsonContent : StringContent
    {
        public JsonContent(string content) 
            : base(content, Encoding.UTF8, "application/json")
        {
            
        }
    }
}
