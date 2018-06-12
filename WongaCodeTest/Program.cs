using EasyNetQ;
using EasyNetQ.Management.Client;
using EasyNetQ.Management.Client.Model;
using Messages;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace WongaCodeTestPublisher
{
    class Program
    {
        public static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                // Do any async anything you need here without worry
                await PublishMessage();
            }).GetAwaiter().GetResult();

            using (var bus = RabbitHutch.CreateBus(string.Format("host={0};virtualhost={1};username={2};password={3}",
                 "localhost", "wonga_host", "mazizi", "wonga")))
            {
                var input = "";
                Console.WriteLine("Enter a message. 'Quit' to quit.");
                while ((input = Console.ReadLine()) != "Quit")
                {
                    bus.Publish(new TextMessage
                    {
                        Text = $"Hello my name is, {input}"
                    });
                }
            }

                      
        }

        static async Task PublishMessage()
        {
            //var routingKey = "wonga";
            var initial = new ManagementClient("http://localhost", "rabbitmq", "password");

            // first create a new virtual host
            var vhost = await initial.CreateVirtualHostAsync("wonga_host").ConfigureAwait(false);

            // next create a user for that virutal host
            var user = await initial.CreateUserAsync(new UserInfo("mazizi", "wonga").AddTag("administrator"));

            // give the new user all permissions on the virtual host
            await initial.CreatePermissionAsync(new PermissionInfo(user, vhost));

            // now log in again as the new user
            var management = new ManagementClient("http://localhost", user.Name, "wonga");

            // test that everything's OK
            await management.IsAliveAsync(vhost);

            //// create an exchange
            //var exchange = await management.CreateExchangeAsync(new ExchangeInfo("wonga_exchagne", "direct"), vhost);

            //// create a queue
            //var queue = await management.CreateQueueAsync(new QueueInfo("wonga_queue"), vhost);

            //// bind the exchange to the queue
            //await management.CreateBinding(exchange, queue, new BindingInfo(routingKey));

            //var input = "";
            //while ((input = Console.ReadLine()) != "Quit")
            //{
            //    // publish a test message
            //    //using (var bus = RabbitHutch.CreateBus("host=localhost"))
            //    //{
            //    //    bus.Send("wonga_queue", new TextMessage { Text = input });                   
            //    //}
            //    await management.PublishAsync(exchange, new PublishInfo(routingKey,
            //        JsonConvert.SerializeObject(new TextMessage { Text = input })));
            //}

            //using (var bus = RabbitHutch.CreateBus(string.Format("host={0};virtualhost={1};username={2};password={3}",
            //    "localhost", "wonga_host", "mazizi", "wonga")))
            //{
            //    var input = "";
            //    Console.WriteLine("Enter a message. 'Quit' to quit.");
            //    while ((input = Console.ReadLine()) != "Quit")
            //    {
            //        bus.Publish(new TextMessage
            //        {
            //            Text = input
            //        });
            //    }
            //}
        }
    }
}
