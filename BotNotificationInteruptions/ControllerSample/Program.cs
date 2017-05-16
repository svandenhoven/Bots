using BotSample.MessageRouting;
using ControllerSample.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace ControllerSample
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var key = ConfigurationManager.AppSettings["BotKey"];
            NotificationSender notificationSender = new NotificationSender(key);

            IList<Party> partiesToNotify = new List<Party>();

            var party = PartyManager.GetParties()[0];
            partiesToNotify.Add(party);

            Console.WriteLine("Write Notification. Your Message, High|Normal|Low");
            Console.Write("> ");
            var messageLine = Console.ReadLine();
            var message = messageLine.Split(',');
            while(message[0].ToLower() != "stop")
            {
                Log($"Sending notification {message[0]}");

                Microsoft.Bot.Connector.DirectLine.ResourceResponse resourceResponse =
                    await notificationSender.NotifyAsync(partiesToNotify, $"{message[0]}",message[1].ToLower().Trim());

                Log($"{((resourceResponse == null) ? "Received no response" : $"Received resource response with ID {resourceResponse.Id}")}");

                Console.Write("> ");
                messageLine = Console.ReadLine();
                message = messageLine.Split(',');
#if DEBUG
                // The following will dump the activity info into Output (console)
                Microsoft.Bot.Connector.DirectLine.Activity activity = await notificationSender.GetLatestReplyAsync();
#endif
            }

            Log("Done");
        }

        static void Log(string message)
        {
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
