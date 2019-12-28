using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetBook.Controllers
{
    public class TestController : Controller
    {
        [HttpGet("api/v1/users")]
        public IActionResult Get()
        {
            return Ok(new { name = "SHREE GANESHA!!!" });
        }
    }
}
