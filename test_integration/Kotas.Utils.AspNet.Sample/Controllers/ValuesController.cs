using System;
using System.Collections.Generic;
using Kotas.Utils.AspNet.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Kotas.Utils.AspNet.Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
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

        [HttpGet("exception")]
        public ActionResult<string> GetException()
        {
            throw new BadInputException("Some bad input provided, please fix it!");
        }

        [HttpGet("exception-global")]
        public ActionResult<string> GetExceptionGlobal()
        {
            throw new Exception("Something wrong, sorry!");
        }

        [HttpGet("exception-unauthorized")]
        public ActionResult<string> GetExceptionUnauthorized()
        {
            throw new UnauthorizedException("Token is invalid");
        }

        [HttpGet("exception-custom")]
        public ActionResult<string> GetExceptionCustom()
        {
            throw new ApplicationException();
        }
    }
}
