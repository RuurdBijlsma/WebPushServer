using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebPushNotification.Models;

namespace WebPushNotification.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SubscribeController : Controller
    {
        // GET api/subscribe
        [HttpGet]
        public string Get()
        {
            Console.WriteLine("New subscription, returning public key");

            return PushServer.Details.PublicKey;
        }

        // POST api/subscribe
        [HttpPost]
        public StatusCodeResult Post([FromBody] JObject subscription)
        {
            Console.WriteLine("Receiving subscription details");

            try
            {
                var endPoint = subscription["endpoint"].ToString();
                var authentication = subscription["keys"]["auth"].ToString();
                var cryptoKey = subscription["keys"]["p256dh"].ToString();

                PushServer.AddClient(new PushClient(endPoint, cryptoKey, authentication));

                return Ok();
            }
            catch
            {
                return new StatusCodeResult(400);
            }
        }
    }
}