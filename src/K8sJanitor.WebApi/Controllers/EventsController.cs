using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using K8sJanitor.WebApi.Models.ExternalEvents;

namespace K8sJanitor.WebApi.Controllers
{
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly  IEventHandler<CapabilityRegisteredEvent> _teamCreatedEventHandler;

        public EventsController( IEventHandler<CapabilityRegisteredEvent> teamCreatedEventHandler)
        {
            _teamCreatedEventHandler = teamCreatedEventHandler;
        }

        [HttpPost("")]
        public async Task AddEvent([FromBody] Newtonsoft.Json.Linq.JObject jObject)
        {
            var teamEvent = jObject.ToObject<CapabilityRegisteredEvent>();

            await _teamCreatedEventHandler.HandleAsync(teamEvent);
        }
    }
}