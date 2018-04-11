using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebPushNotification.Models;

namespace WebPushNotification.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UnsubscribeController : Controller
    {
        // POST api/unsubscribe
        [HttpPost]
        public StatusCodeResult Post([FromBody] JObject subscription)
        {
            Console.WriteLine("Unsubscribe, Receiving subscription details");

            try
            {
                var endPoint = subscription["endpoint"].ToString();
                var authentication = subscription["keys"]["auth"].ToString();
                var cryptoKey = subscription["keys"]["p256dh"].ToString();

                PushServer.RemoveClient(PushServer.FindClient(endPoint, authentication, cryptoKey));

                return Ok();
            }
            catch
            {
                return new StatusCodeResult(400);
            }
        }
    }
}