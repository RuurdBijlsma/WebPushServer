using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebPushNotification.Models;

namespace WebPushNotification.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SendNotificationController : Controller
    {
        // POST api/unsubscribe
        [HttpGet("{title}")]
        public StatusCodeResult Get(string title)
        {
            Console.WriteLine("Manually sending message: " + title);
            PushServer.SendToAll(new Notification
            {
                Title = title,
                Message = "Manual notification",
                Icon = "icon.png"
            });

            return Ok();
        }
    }
}