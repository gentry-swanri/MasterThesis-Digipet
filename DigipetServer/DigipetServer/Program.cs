using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Newtonsoft.Json;

namespace DigipetServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Server");

            PlayerManagement playerManagement = new PlayerManagement();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // create if not exist and bind exchange and queue for request and response
                channel.ExchangeDeclare(exchange: "DigipetExchange",
                                        type: "direct",
                                        durable: true);

                channel.QueueDeclare(queue: "DigipetQueue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false);

                channel.QueueBind(queue: "DigipetQueue",
                                  exchange: "DigipetExchange",
                                  routingKey: "DigipetRoutingKey");

                Console.WriteLine(" [*] Waiting for messages.");

                // create consumer and tell consumer to waiting request from requestQueue
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "DigipetQueue",
                                     noAck: false,
                                     consumer: consumer);

                consumer.Received += (model, ea) =>
                {
                    // receive message from queue
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    // process request
                    //Console.WriteLine(" [x] {0}", message);
                    dynamic msg = JsonConvert.DeserializeObject(message);
                    if (msg != null)
                    {
                        if (msg["type"] == 1)
                        {
                            Console.WriteLine(" [x] processing request from {0} ", msg["id"]);

                            ResponseJson responseJson = new ResponseJson();
                            responseJson.id = msg["id"];
                            responseJson.type = 2;
                            responseJson.mapData = playerManagement.AcquireMapData(msg);
                            responseJson.needCreateMap = playerManagement.GetNeedCreateMap();
                            responseJson.playerName = msg["playerName"];
                            responseJson.playerPosX = playerManagement.GetPlayerPosX();
                            responseJson.playerPosY = playerManagement.GetPlayerPosY();

                            var jsonString = JsonConvert.SerializeObject(responseJson);

                            // send response
                            Console.WriteLine(" [x] sending response to {0} ", msg["id"]);

                            var newMessage = Encoding.UTF8.GetBytes(jsonString);
                            channel.BasicPublish(exchange: "DigipetExchange",
                                                 routingKey: "DigipetRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage);
                        }

                        if (msg["type"] == 3)
                        {
                            playerManagement.SetPlayerActive(msg);
                        }
                    }

                    channel.BasicAck(ea.DeliveryTag, false);
                };

                Console.WriteLine(" Press [enter] to stop server and exit.");
                Console.ReadLine();
            }
        }

        class ResponseJson
        {
            public string id;
            public int type;
            public dynamic mapData;
            public bool needCreateMap;
            public string playerName;
            public float playerPosX;
            public float playerPosY;
        }
    }
}
