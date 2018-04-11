using System;
using Newtonsoft.Json;
using WebPush;

namespace WebPushNotification.Models
{
    public class PushClient
    {
        public PushClient(string endPoint, string cryptoKey, string authentication)
        {
            EndPoint = endPoint;
            CryptoKey = cryptoKey;
            Authentication = authentication;
            PushSubscription = new PushSubscription(EndPoint, CryptoKey, Authentication);
        }

        private WebPushClient WebPushClient { get; } = new WebPushClient();
        private PushSubscription PushSubscription { get; }
        public string EndPoint { get; }
        public string CryptoKey { get; }
        public string Authentication { get; }

        /// <summary>
        /// Send notification to this client
        /// </summary>
        /// <param name="vapidDetails">Vapid key details of the server, this is required so the client can verify this server sent the message</param>
        /// <param name="notification">The notification which should be sent</param>
        public async void Send(VapidDetails vapidDetails, Notification notification)
        {
            var message = JsonConvert.SerializeObject(notification);
            try
            {
                await WebPushClient.SendNotificationAsync(PushSubscription, message, vapidDetails);
            }
            catch (WebPushException e)
            {
                // Error occurs when user has blocked the notification permission
                // Or if the user have unsubscribed from the push service manually from console
                Console.WriteLine("Error sending web push, status code: " + e.StatusCode);
                
                PushServer.RemoveClient(this);
            }
        }
    }
}