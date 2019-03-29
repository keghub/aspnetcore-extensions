using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMG.Extensions.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Error.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            throw new InvalidOperationException("You really shouldn't be doing this!");
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            try
            {
                throw new InvalidOperationException("You really shouldn't be doing this!");
            }
            catch (Exception ex)
            {
                var eventId = new EventId(42, "A serious error");

                ex.DescribeError(eventId, new { id }, (s, e) => $"Something really bad happened when fetching value with id {s.id}");

                throw;
            }
        }
    }
}
