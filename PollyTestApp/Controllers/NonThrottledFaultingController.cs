using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PollyTestApp.Models;

namespace PollyTestApp.Controllers
{

    public class NonThrottledFaultingController : ControllerBase
    {
        readonly TimeSpan delay = TimeSpan.FromSeconds(20);

        // GET api/values
        /// <summary>
        /// Returns a simple string array of values.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("NonThrottledFaulting/Get")]
        public IEnumerable<string> Get()
        {
            // Fault, by returning the result slowly.
            Thread.Sleep(delay);
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        /// <summary>
        /// Accepts a simple integer parameter, which is written back to the
        /// response in the form of a simple message.  Faults by delaying for a 
        /// fixed period, before responding.
        /// </summary>
        /// <param name="id">Integer value that gets returned in the response
        /// message.</param>
        /// <returns>Returns: "Response from server to request #{id}"</returns>
        [HttpGet, Route("NonThrottledFaulting/Get/{id}")]
        public Message Get(int id)
        {
            // Fault, by returning the result slowly.
            Thread.Sleep(delay);
            return new Message { Text = "Slow, faulting response from server to request #" + id };
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
