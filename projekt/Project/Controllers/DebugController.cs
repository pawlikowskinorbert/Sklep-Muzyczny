using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Project.Controllers
{
    public class DebugController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public DebugController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public IActionResult GetSession()
        {
            var sessionId = _contextAccessor.HttpContext?.Session?.Id;
            var sessionData = _contextAccessor.HttpContext?.Session.Keys.ToDictionary(
                key => key,
                key => _contextAccessor.HttpContext.Session.GetString(key));

            return Ok(new
            {
                SessionId = sessionId,
                SessionData = sessionData
            });
        }
    }
}