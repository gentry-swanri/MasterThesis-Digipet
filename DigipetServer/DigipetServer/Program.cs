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

            MySQLDatabase db = new MySQLDatabase();
            PlayerManagement playerManagement = new PlayerManagement();

            ConnectionFactory factory = new ConnectionFactory();
            //factory.HostName = "localhost";
            factory.HostName = "";
            factory.Port = 5672;
            factory.UserName = "";
            factory.Password = "";
            factory.VirtualHost = "";
            
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // create if not exist and bind exchange and queue for request and response
                channel.ExchangeDeclare(exchange: "DigipetRequestExchange",
                                        type: "direct",
                                        durable: true);

                channel.ExchangeDeclare(exchange: "DigipetResponseExchange",
                                        type: "direct",
                                        durable: true);

                var queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: queueName,
                                  exchange: "DigipetRequestExchange",
                                  routingKey: "DigipetRequestRoutingKey");

                Console.WriteLine(" [*] Waiting for messages.");

                // create consumer and tell consumer to waiting request from requestQueue
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    // receive message from queue
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    dynamic msg = JsonConvert.DeserializeObject(message);
                    if (msg != null)
                    {
                        if (msg["type"] == "login")
                        {
                            Console.WriteLine(" [x] processing login request from {0} ", msg["username"]);

                            string username = (string)msg["username"];
                            string password = (string)msg["password"];
                            int count = db.Count("user", "username='" + username + "' AND password='" + password + "' AND is_active=0");
                            
                            if (count == 1)
                            {
                                count = db.Update("user", "is_active=1", "username='" + username + "'");
                            }

                            playerManagement.AddPlayer(username);

                            LoginResponseJson response = new LoginResponseJson();
                            response.id = msg["id"];
                            response.type = msg["type"];
                            response.count = count;
                            response.username = username;
                            response.data = db.GetPetData(username);

                            var jsonString = JsonConvert.SerializeObject(response);

                            //Console.WriteLine(jsonString);

                            // send response
                            var newMessage = Encoding.UTF8.GetBytes(jsonString);
                            channel.BasicPublish(exchange: "DigipetResponseExchange",
                                                 routingKey: "DigipetResponseRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage);

                            Console.WriteLine(" [x] login response sent to {0} ", msg["username"]);
                        }

                        if (msg["type"] == "newaccount")
                        {
                            Console.WriteLine(" [x] processing new account request from {0} ", msg["username"]);

                            string firstName = (string)msg["firstName"];
                            string lastName = (string)msg["lastName"];
                            string email = (string)msg["email"];
                            string username = (string)msg["username"];
                            string petName = (string)msg["petName"];
                            string password = (string)msg["password"];

                            NewAccountResponseJson response = new NewAccountResponseJson();
                            response.id = msg["id"];
                            response.type = msg["type"];
                            response.result = db.CreateNewAccount(firstName, lastName, email, username, petName, password);

                            var jsonString = JsonConvert.SerializeObject(response);

                            // send response
                            var newMessage = Encoding.UTF8.GetBytes(jsonString);
                            channel.BasicPublish(exchange: "DigipetResponseExchange",
                                                 routingKey: "DigipetResponseRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage);

                            Console.WriteLine(" [x] new account response sent to {0} ", msg["username"]);
                        }

                        if (msg["type"] == "map")
                        {
                            Console.WriteLine(" [x] processing map request from {0} ", msg["playerName"]);

                            ResponseJson responseJson = new ResponseJson();
                            responseJson.id = msg["id"];
                            responseJson.type = msg["type"];
                            responseJson.mapData = playerManagement.AcquireMapData(msg);
                            responseJson.needCreateMap = playerManagement.GetNeedCreateMap();
                            responseJson.playerName = msg["playerName"];
                            responseJson.playerPosX = playerManagement.GetPlayerPosX();
                            responseJson.playerPosY = playerManagement.GetPlayerPosY();
                            responseJson.centerPosX = playerManagement.GetCenterPosX();
                            responseJson.centerPosY = playerManagement.GetCenterPosY();
                            responseJson.tileX = playerManagement.GetTileX();
                            responseJson.tileY = playerManagement.GetTileY();

                            var jsonString = JsonConvert.SerializeObject(responseJson);

                            // send response
                            var newMessage = Encoding.UTF8.GetBytes(jsonString);
                            channel.BasicPublish(exchange: "DigipetResponseExchange",
                                                 routingKey: "DigipetResponseRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage);

                            Console.WriteLine(" [x] map response sent to {0} ", msg["playerName"]);


                            // send update to other user that user change position
                            List<UnityPlayerPetPosition> unityPlayerPos = playerManagement.GetOthers(playerManagement.GetTileX(), playerManagement.GetTileY());
                            UnityPlayerPetPosition curPlayer = unityPlayerPos.Find(x => x.playerName == (string)msg["playerName"]);
                            curPlayer.petLastPosX += playerManagement.GetCenterPosX();
                            curPlayer.petLastPosY += playerManagement.GetCenterPosY();
                            curPlayer.petPosX += playerManagement.GetCenterPosX();
                            curPlayer.petPosY += playerManagement.GetCenterPosY();

                            ListPlayerResponseJson listPlayer = new ListPlayerResponseJson();
                            listPlayer.unityPlayerPos = unityPlayerPos;
                            listPlayer.type = "listplayer";
                            listPlayer.tileX = playerManagement.GetTileX();
                            listPlayer.tileY = playerManagement.GetTileY();

                            var jsonString2 = JsonConvert.SerializeObject(listPlayer);

                            // send response
                            //Console.WriteLine(" list player2 = " + jsonString2);

                            var newMessage2 = Encoding.UTF8.GetBytes(jsonString2);
                            channel.BasicPublish(exchange: "DigipetResponseExchange",
                                                 routingKey: "DigipetResponseRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage2);
                            
                        }

                        if (msg["type"] == "maptohome")
                        {
                            Console.WriteLine(" [x] processing maptohome request from {0} ", msg["id"]);

                            int result = playerManagement.SetPlayerActiveFalse((string)msg["username"], false);


                            // send update to other user that user return home
                            List<UnityPlayerPetPosition> unityPlayerPos = playerManagement.GetOthers((int)msg["tileX"], (int)msg["tileY"]);

                            ListPlayerResponseJson listPlayer = new ListPlayerResponseJson();
                            listPlayer.unityPlayerPos = unityPlayerPos;
                            listPlayer.type = "listplayer";
                            listPlayer.tileX = (int)msg["tileX"];
                            listPlayer.tileY = (int)msg["tileY"];

                            var jsonString2 = JsonConvert.SerializeObject(listPlayer);

                            // send response
                            //Console.WriteLine(" list player2 = " + jsonString2);

                            var newMessage2 = Encoding.UTF8.GetBytes(jsonString2);
                            channel.BasicPublish(exchange: "DigipetResponseExchange",
                                                 routingKey: "DigipetResponseRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage2);



                            // map to home response
                            PauseRespondJson respond = new PauseRespondJson();
                            respond.id = msg["id"];
                            respond.type = msg["type"];
                            respond.result = result;

                            var jsonString = JsonConvert.SerializeObject(respond);

                            // send response
                            var newMessage = Encoding.UTF8.GetBytes(jsonString);
                            channel.BasicPublish(exchange: "DigipetResponseExchange",
                                                 routingKey: "DigipetResponseRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage);

                            Console.WriteLine(" [x] maptohome response sent to {0} ", msg["id"]);

                        }

                        if (msg["type"] == "route")
                        {
                            Console.WriteLine(" [x] processing route request from {0} ", msg["id"]);

                            List<Coordinate> route = playerManagement.PlayerRouting(msg);

                            RouteResponseJson routeResponseJson = new RouteResponseJson();
                            routeResponseJson.route = route;
                            routeResponseJson.type = msg["type"];
                            routeResponseJson.id = msg["id"];

                            var jsonString = JsonConvert.SerializeObject(routeResponseJson);

                            // send response
                            var newMessage = Encoding.UTF8.GetBytes(jsonString);
                            channel.BasicPublish(exchange: "DigipetResponseExchange",
                                                 routingKey: "DigipetResponseRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage);

                            Console.WriteLine(" [x] route response sent to {0} ", msg["id"]);
                        }

                        if (msg["type"] == "returntitle")
                        {
                            Console.WriteLine(" [x] processing return title request from {0} ", msg["username"]);

                            string username = (string)msg["username"];
                            int rest = (int)msg["rest"];
                            int energy = (int)msg["energy"];
                            int agility = (int)msg["agility"];
                            int stress = (int)msg["stress"];
                            int heart = (int)msg["heart"];
                            int money = (int)msg["money"];
                            int xp = (int)msg["xp"];

                            int result = db.UpdateStatus(username, rest, energy, agility, stress, heart, money, xp);
                            
                            if (result == 1)
                            {
                                result = playerManagement.RemovePlayer(msg);
                            }
                            
                            ReturnTitleRespondJson respond = new ReturnTitleRespondJson();
                            respond.id = msg["id"];
                            respond.type = msg["type"];
                            respond.result = result;

                            var jsonString = JsonConvert.SerializeObject(respond);

                            // send response
                            var newMessage = Encoding.UTF8.GetBytes(jsonString);
                            channel.BasicPublish(exchange: "DigipetResponseExchange",
                                                 routingKey: "DigipetResponseRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage);

                            Console.WriteLine(" [x] return title response sent to {0} ", msg["username"]);
                        }

                        if (msg["type"] == "listplayer")
                        {
                            Console.WriteLine(" [x] processing list player request from {0} ", msg["username"]);

                            playerManagement.UpdatePetLocation(msg);
                            List<UnityPlayerPetPosition> unityPlayerPos = playerManagement.GetOthers((int)msg["tileX"], (int)msg["tileY"]);

                            ListPlayerResponseJson listPlayer = new ListPlayerResponseJson();
                            listPlayer.unityPlayerPos = unityPlayerPos;
                            listPlayer.type = msg["type"];
                            listPlayer.tileX = (int)msg["tileX"];
                            listPlayer.tileY = (int)msg["tileY"];

                            var jsonString = JsonConvert.SerializeObject(listPlayer);

                            // send response
                            Console.WriteLine(" [x] sending list player response to {0} ", msg["username"]);

                            var newMessage = Encoding.UTF8.GetBytes(jsonString);
                            channel.BasicPublish(exchange: "DigipetResponseExchange",
                                                 routingKey: "DigipetResponseRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage);

                        }

                        if (msg["type"] == "updateBall")
                        {
                            Console.WriteLine(" [x] processing ball state request from {0} ", msg["username"]);

                            float[] ballPos = playerManagement.UpdateBallState(msg);
                            
                            UpdateBallResponse response = new UpdateBallResponse();
                            response.type = msg["type"];
                            response.username = msg["username"];
                            response.tileX = msg["tileX"];
                            response.tileY = msg["tileY"];
                            
                            response.ballPosX = ballPos[0];
                            response.ballPosY = msg["ballPosY"];
                            response.ballPosZ = ballPos[1];
                            
                            response.ballState = msg["ballState"];

                            var jsonString = JsonConvert.SerializeObject(response);

                            Console.WriteLine(" [x] sending ball state response to {0} ", msg["username"]);

                            var newMessage = Encoding.UTF8.GetBytes(jsonString);
                            channel.BasicPublish(exchange: "DigipetResponseExchange",
                                                    routingKey: "DigipetResponseRoutingKey",
                                                    basicProperties: null,
                                                    body: newMessage);
                            
                        }

                        if (msg["type"] == "resetpassword")
                        {
                            Console.WriteLine(" [x] processing reset password request from {0} ", msg["id"]);

                            EmailManagement emailManage = new EmailManagement();
                            int user_id = -1;
                            int updateResult = -1;
                            int result = -1;

                            string email = (string)msg["email"];

                            user_id = db.FindUserIdByEmail(email);
                            if (user_id != -1)
                            {
                                string newPass = emailManage.CreateRandomPassword(6);
                                updateResult = db.Update("user", "password="+newPass, "id="+user_id);
                                if (updateResult != -1)
                                {
                                    result = emailManage.SendEmail(email, newPass, 1);
                                }
                            }

                            ResetPasswordJson reset = new ResetPasswordJson();
                            reset.id = msg["id"];
                            reset.type = msg["type"];
                            reset.result = result;

                            var jsonString = JsonConvert.SerializeObject(reset);

                            // send response
                            Console.WriteLine(" [x] sending reset password response to {0} ", msg["id"]);

                            var newMessage = Encoding.UTF8.GetBytes(jsonString);
                            channel.BasicPublish(exchange: "DigipetResponseExchange",
                                                 routingKey: "DigipetResponseRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage);
                        }

                        if (msg["type"] == "changepassword")
                        {
                            Console.WriteLine(" [x] processing change password request from {0} ", msg["username"]);

                            EmailManagement emailManage = new EmailManagement();

                            string username = (string)msg["username"];
                            string oldPass = (string)msg["oldPass"];
                            string newPass = (string)msg["newPass"];

                            int result = -1;

                            int count = db.Count("user", "username='"+username+"'");
                            if (count != -1)
                            {
                                int count2 = db.Count("user", "password="+oldPass);
                                if (count2 != -1)
                                {
                                    result = db.Update("user", "password="+newPass, "username='"+username+"'");
                                    if (result != -1)
                                    {
                                        string email = db.FindEmail(username);
                                        result = emailManage.SendEmail(email, "", 2);
                                    }
                                }
                            } 

                            ChangePasswordJson change = new ChangePasswordJson();
                            change.id = msg["id"];
                            change.type = msg["type"];
                            change.result = result;

                            var jsonString = JsonConvert.SerializeObject(change);

                            // send response
                            Console.WriteLine(" [x] sending change password response to {0} ", msg["username"]);

                            var newMessage = Encoding.UTF8.GetBytes(jsonString);
                            channel.BasicPublish(exchange: "DigipetResponseExchange",
                                                 routingKey: "DigipetResponseRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage);
                        }

                        if (msg["type"] == "restart")
                        {
                            Console.WriteLine(" [x] processing restart request from {0} ", msg["id"]);

                            string username = (string)msg["username"];

                            RestartJson response = new RestartJson();
                            response.id = msg["id"];
                            response.type = msg["type"];
                            response.result = db.UpdateStatus(username, 100, 100, 100, 100, 6, 100, 0);

                            var jsonString = JsonConvert.SerializeObject(response);

                            // send response
                            var newMessage = Encoding.UTF8.GetBytes(jsonString);
                            channel.BasicPublish(exchange: "DigipetResponseExchange",
                                                 routingKey: "DigipetResponseRoutingKey",
                                                 basicProperties: null,
                                                 body: newMessage);

                            Console.WriteLine(" [x] restart response sent to {0} ", msg["id"]);
                        }
                    }
                    

                    channel.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(queue: queueName,
                                     noAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to stop server and exit.");
                Console.ReadLine();
            }
        }

        class ResponseJson
        {
            public string id;
            public string type;
            public bool needCreateMap;
            public string playerName;
            public float playerPosX;
            public float playerPosY;
            public float centerPosX;
            public float centerPosY;
            public int tileX;
            public int tileY;
            public ListMapData mapData;
        }

        class RouteResponseJson
        {
            public List<Coordinate> route;
            public string type;
            public string id;
        }

        class ListPlayerResponseJson
        {
            public List<UnityPlayerPetPosition> unityPlayerPos;
            public string type;
            public int tileX;
            public int tileY;
        }

        class LoginResponseJson
        {
            public string id;
            public string type;
            public string username;
            public int count;
            public List<Object> data;
        }

        class NewAccountResponseJson
        {
            public string id;
            public string type;
            public int result;
        }

        class PauseRespondJson
        {
            public string id;
            public string type;
            public int result;
        }

        class ReturnTitleRespondJson
        {
            public string id;
            public string type;
            public int result;
        }

        class ResetPasswordJson
        {
            public string id;
            public string type;
            public int result;
        }

        class ChangePasswordJson
        {
            public string id;
            public string type;
            public int result;
        }

        class RestartJson
        {
            public string id;
            public string type;
            public int result;
        }

        class UpdateBallResponse
        {
            public string type;
            public string username;
            public int tileX;
            public int tileY;
            public float ballPosX;
            public float ballPosY;
            public float ballPosZ;
            public string ballState;
        }
        
    }
}
