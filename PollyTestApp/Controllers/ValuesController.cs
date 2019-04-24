using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace PollyTestApp.Controllers
{
    
    public class ValuesController : ControllerBase
    {
        // GET api/values
        /// <summary>
        /// Returns a simple string array of values.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/values")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        /// <summary>
        /// Accepts a simple integer parameter, which is written back to the
        /// response in the form of a simple message.
        /// </summary>
        /// <param name="id">Integer value that gets returned in the response
        /// message.</param>
        /// <returns>Returns: "Response from server to request #{id}"</returns>
        [HttpGet, Route("api/values/{id}")]
        public Models.Message Get(int id)
        {
            var msg = new Models.Message();
            msg.Text = "Response from server to request #" + id.ToString();
            return msg;
        }
        [HttpGet]
        [Route("")]
        public ContentResult Echo()
        {
            return new ContentResult { Content = "ok", ContentType = "text/plain", StatusCode = 200 };
        }
        //// POST api/values
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //public void Delete(int id)
        //{
        //}
    }
}
