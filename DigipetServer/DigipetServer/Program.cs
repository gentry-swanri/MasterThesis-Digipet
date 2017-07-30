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
                            Console.WriteLine(" [x] processing map request from {0} ", msg["id"]);

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
                            Console.WriteLine(" [x] sending map response to {0} ", msg["id"]);

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

                        if (msg["type"] == 4)
                        {
                            Console.WriteLine(" [x] processing route request from {0} ", msg["id"]);

                            List<Coordinate> route = playerManagement.PlayerRouting(msg);

                            RouteResponseJson routeResponseJson = new RouteResponseJson();
                            routeResponseJson.route = route;
                            routeResponseJson.type = 5;
                            routeResponseJson.id = msg["id"];

                            var jsonString = JsonConvert.SerializeObject(routeResponseJson);

                            // send response
                            Console.WriteLine(" [x] sending route response to {0} ", msg["id"]);

                            var newMessage = Encoding.UTF8.GetBytes(jsonString);
                            channel.BasicPublish(exchange: "DigipetExchange",
                                                 routingKey: "DigipetRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage);
                        }

                        if (msg["type"] == 6)
                        {
                            Console.WriteLine(" [x] processing list player request from {0} ", msg["id"]);

                            List<UnityPlayerPosition> unityPlayerPos = playerManagement.GetOthersInRange(msg, 30.0f);

                            ListPlayerResponseJson listPlayer = new ListPlayerResponseJson();
                            listPlayer.unityPlayerPos = unityPlayerPos;
                            listPlayer.type = 7;
                            listPlayer.id = msg["id"];

                            var jsonString = JsonConvert.SerializeObject(listPlayer);

                            // send response
                            Console.WriteLine(" [x] sending list player response to {0} ", msg["id"]);

                            var newMessage = Encoding.UTF8.GetBytes(jsonString);
                            channel.BasicPublish(exchange: "DigipetExchange",
                                                 routingKey: "DigipetRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage);

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

        class RouteResponseJson
        {
            public List<Coordinate> route;
            public int type;
            public int id;
        }

        class ListPlayerResponseJson
        {
            public List<UnityPlayerPosition> unityPlayerPos;
            public int type;
            public int id;
        }
    }
}
