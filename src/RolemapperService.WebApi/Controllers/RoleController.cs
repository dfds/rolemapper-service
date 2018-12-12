using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RolemapperService.WebApi.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        [HttpGet("")]
        public ActionResult<string> Ping()
        {
            return Ok("OK");
        }
    }
}
