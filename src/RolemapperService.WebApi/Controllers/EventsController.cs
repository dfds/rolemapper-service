using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RolemapperService.WebApi.Models.ExternalEvents;

namespace RolemapperService.WebApi.Controllers
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