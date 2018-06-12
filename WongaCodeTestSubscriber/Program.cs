using EasyNetQ;
using EasyNetQ.Management.Client;
using EasyNetQ.Management.Client.Model;
using Messages;
using System;
using System.Threading.Tasks;

namespace WongaCodeTestSubscriber
{
    class Program
    {

        private const string testVHost = "wonga_host";

        public static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                // Do any async anything you need here without worry
                await Subscribe();
            }).GetAwaiter().GetResult();

            using (var bus = RabbitHutch.CreateBus(string.Format("host={0};virtualhost={1};username={2};password={3}",
                "localhost", "wonga_host", "mazizi", "wonga")))
            {
                bus.Subscribe<TextMessage>("test", HandleTextMessage);

                Console.WriteLine("Listening for messages. Hit <return> to quit.");
                Console.ReadLine();
            }


        }

        static async Task Subscribe()
        {

            var management = new ManagementClient("http://localhost", "mazizi", "wonga");

            // get virtual host
            var vhost = await management.GetVhostAsync(testVHost).ConfigureAwait(false);

            // test that everything's OK
            await management.IsAliveAsync(vhost);

            //// create a queue
            //var queue = await management.GetQueueAsync("wonga_queue", vhost);

            //// get any messages on the queue
            //var messages = await management.GetMessagesFromQueueAsync(queue, new GetMessagesCriteria(1, false)).ConfigureAwait(false);

            //foreach (var message in messages)
            //{
            //    Console.Out.WriteLine("Hello {0}, i am your father!", message.Payload);
            //}

            //using (var bus = RabbitHutch.CreateBus(string.Format("host={0};virtualhost={1};username={2};password={3}",
            //    "localhost","wonga_host","mazizi","wonga")))
            //{
            //    bus.Subscribe<TextMessage>("test", HandleTextMessage);

            //    Console.WriteLine("Listening for messages. Hit <return> to quit.");
            //    Console.ReadLine();
            //}

        }

        static void HandleTextMessage(TextMessage textMessage)
        {
            if (string.IsNullOrEmpty(textMessage.Text.Split(',')[1]))
                Console.WriteLine("No name was recieved");
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Hello {0}, I am your father!", textMessage.Text.Split(',')[1]);
                Console.ResetColor();
            }
        }
    }
}
