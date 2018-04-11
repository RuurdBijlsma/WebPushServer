using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebPush;

namespace WebPushNotification.Models
{
    public static class PushServer
    {
        private const string VapidDetailsFile = "vapidDetails.json";
        private const string ClientsFile = "clients.json";
        private const string Subject = "mailto:mail@ruurdbijlsma.com";
        private static List<PushClient> Clients { get; set; }
        public static VapidDetails Details { get; private set; }

        /// <summary>
        /// Send notification to all clients
        /// </summary>
        /// <param name="notification">Notification to be sent</param>
        public static void SendToAll(Notification notification)
        {
            Clients.ForEach(client => client.Send(Details, notification));
        }

        /// <summary>
        /// Find client based on endpoint, authentication and cryptokey
        /// </summary>
        /// <returns>PushClient that matches filter</returns>
        public static PushClient FindClient(string endPoint, string authentication, string cryptoKey)
        {
            return Clients.SingleOrDefault(client =>
                client.Authentication == authentication &&
                client.CryptoKey == cryptoKey &&
                client.EndPoint == endPoint
            );
        }

        /// <summary>
        /// Remove a client from the server
        /// </summary>
        /// <param name="client">Client to be removed</param>
        public static async void RemoveClient(PushClient client)
        {
            Clients.Remove(client);
            await ExportClients(Clients);
        }

        /// <summary>
        /// Add a client to the server
        /// </summary>
        /// <param name="client">Client to be added</param>
        public static async void AddClient(PushClient client)
        {
            client.Send(Details, new Notification
            {
                Title = "Yes!",
                Message = "Subscribe succesful",
                Icon = "icon.png"
            });
            Clients.Add(client);
            await ExportClients(Clients);
        }

        /// <summary>
        /// Start the push server, this initialized vapid details from a file, if it exists else it will create the vapid details
        /// </summary>
        public static async void Start()
        {
            Console.WriteLine("Started push server");
            // Read Vapid details from cache or create new ones
            if (File.Exists(VapidDetailsFile))
            {
                Details = await ImportVapidDetails();
            }
            else
            {
                Details = VapidHelper.GenerateVapidKeys();
                Details.Subject = Subject;
                await ExportVapidDetails(Details);
            }

            // Read list of clients from cache or create empty list
            if (File.Exists(ClientsFile))
            {
                Clients = await ImportClients();
            }
            else
            {
                Clients = new List<PushClient>();
                await ExportClients(Clients);
            }
        }

        /// <summary>
        /// Retrieves vapid details from file
        /// </summary>
        /// <returns>Vapid details containing public and private keys</returns>
        private static async Task<VapidDetails> ImportVapidDetails()
        {
            // Vapid details have to be persistantly cached to make sure the private key stays the same
            var jsonString = await File.ReadAllTextAsync(VapidDetailsFile);
            return JsonConvert.DeserializeObject<VapidDetails>(jsonString);
        }

        /// <summary>
        /// Save vapid details to file
        /// </summary>
        /// <param name="details">The vapid details that should be saved to file</param>
        /// <returns>Task that completes when saving is done</returns>
        private static async Task ExportVapidDetails(VapidDetails details)
        {
            var jsonString = JsonConvert.SerializeObject(details);
            await File.WriteAllTextAsync(VapidDetailsFile, jsonString);
        }

        /// <summary>
        /// Retrieves clients information from file
        /// </summary>
        /// <returns>A list of clients, each client contains endpoints and keys</returns>
        private static async Task<List<PushClient>> ImportClients()
        {
            // In production this would read from a database
            var jsonString = await File.ReadAllTextAsync(ClientsFile);
            var result = JsonConvert.DeserializeObject<List<PushClient>>(jsonString);

            return result;
        }

        /// <summary>
        /// Save client details to file
        /// </summary>
        /// <param name="clients">List of clients that should be saved</param>
        /// <returns>Task that completes when saving is done</returns>
        private static async Task ExportClients(List<PushClient> clients)
        {
            // In production this would save to a database in a user table
            var jsonString = JsonConvert.SerializeObject(clients);
            await File.WriteAllTextAsync(ClientsFile, jsonString);
        }
    }
}