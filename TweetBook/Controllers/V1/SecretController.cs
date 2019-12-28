using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Filters;

namespace TweetBook.Controllers.V1
{
    [ApiKeyAuth]
    public class SecretController : ControllerBase
    {
        [HttpGet("Secret")]
        public IActionResult GetSecret()
        {
            return Ok("I have a great secret!!!");
        }
    }
}
