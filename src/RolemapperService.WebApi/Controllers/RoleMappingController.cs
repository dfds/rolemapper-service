using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RolemapperService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleMappingController : ControllerBase
    {
        [HttpGet("")]
        public ActionResult<string> Map()
        {
            return Ok("OK");
        }
    }
}
