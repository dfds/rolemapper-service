using System.Threading.Tasks;
using K8sJanitor.WebApi.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace K8sJanitor.WebApi.Controllers
{
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly  IEventDispatcher _eventDispatcher;

        public EventsController(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        [HttpPost("")]
        public async Task AddEvent([FromBody] Newtonsoft.Json.Linq.JObject jObject)
        {
            var eventJson = jObject.ToString(Newtonsoft.Json.Formatting.None);

            await _eventDispatcher.Send(eventJson);
        }
    }
}