using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RolemapperService.WebApi.Services;

namespace RolemapperService.WebApi.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IConfigMapService _configMapService;

        public RoleController(IConfigMapService configMapService)
        {
            _configMapService = configMapService;
        }

        [HttpGet("")]
        public ActionResult<string> Ping()
        {
            return Ok("OK");
        }
    }
}
