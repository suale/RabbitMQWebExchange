using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQWebExchange.ServiceModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace RabbitMQWebExchange.Services
{
    public class Consumer
    {
        static ManualResetEvent _mre = new ManualResetEvent(false);
        private readonly RabbitMQService _rabbitMQService;
        public static List<RabbitMessage> rabbitMessages = new List<RabbitMessage>();
        public static List<RabbitMessage> rabbitMessagesClientList = new List<RabbitMessage>();
        public static int sayac = 0;
        public static int sayac2 = 0;
        public Consumer()
        {

            _rabbitMQService = new RabbitMQService();
            

            using (var connection = _rabbitMQService.GetRabbitMQConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

                    var queueName2 = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queue: queueName2,
                                      exchange: "logs",
                                      routingKey: "");


                    var consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(channel);

                    Thread t1 = new Thread(() =>
                    {

                        consumer.Received += (model, ea) =>
                        {

                            _mre.WaitOne();
                            var body = ea.Body.ToArray();
                            
                            var message = Encoding.UTF8.GetString(body);
                            
                            var result = JsonConvert.DeserializeObject<RabbitMessage>(message);
                            rabbitMessages.Add(result);
                            rabbitMessagesClientList.Add(result);

                        };

                    });
                    t1.Start();

                    Thread t2 = new Thread(() =>
                    {
                        while (true)
                        {
                            _mre.Reset();
                            //send messages
                            _mre.Set();
                            Thread.Sleep(1000);

                        }

                    });
                    t2.Start();

                    string consumerTag = channel.BasicConsume(queueName2, true, consumer);
                    Console.ReadLine();
                }

            }
        }
    }
}