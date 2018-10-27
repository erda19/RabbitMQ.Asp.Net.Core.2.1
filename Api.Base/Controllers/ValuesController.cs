using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Microservices.Net.Core.Api.Base.IntegrationEvents.Events;
using Inventory.Microservices.Net.Core.EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ValuesController : ControllerBase
    {

        private readonly IEventBus _eventBus;
        public ValuesController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var a = new { Amount = 123, Message = "Hello" };
            var b = new { Amount = 108, Message = "Hello" };

            if(a.Equals(b))
            {

            }


            return new string[] { "value1", "value2" };


        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
            var eventMessage = new ProductChangedIntegrationEvent(1,"test", "testOld");
            _eventBus.Publish(eventMessage);
        }

        [HttpPost("Post2")]
        public void Post2([FromBody] string value)
        {
            var eventMessage = new ProductAddIntegrationEvent(1, "test", "testOld");
            _eventBus.Publish(eventMessage);
        }
        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
