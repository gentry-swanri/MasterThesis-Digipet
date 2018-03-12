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
    class PlayerManagement
    {
        private List<Player> listPlayer;

        private float playerPosX;
        private float playerPosY;
        private float centerPosX;
        private float centerPosY;
        private int tileX;
        private int tileY;
        private bool needCreateMap;

        // contain list of player position around specific player
        private List<UnityPlayerPetPosition> listPlayerPos;

        public PlayerManagement()
        {
            listPlayer = new List<Player>();
            listPlayerPos = new List<UnityPlayerPetPosition>();
        }

        public ListMapData AcquireMapData(dynamic data)
        {
            playerPosX = float.MinValue;
            playerPosY = float.MinValue;

            Player currentPlayer;
         
            bool registered = listPlayer.Exists(x => x.GetPlayerName() == (string)data.playerName);
            if (!registered)
            {
                currentPlayer = new Player(data.playerName.ToString(), (float)Convert.ToDouble(data.latitude), (float)Convert.ToDouble(data.longitude), data.petName.ToString(), (float)Convert.ToDouble(data.petPosX), (float)Convert.ToDouble(data.petPosY));
                listPlayer.Add(currentPlayer);
            }
            else
            {
                currentPlayer = listPlayer.Find(x => x.GetPlayerName() == (string)data.playerName);
                if (currentPlayer.GetIsActive() == false)
                {
                    currentPlayer.SetIsActive(true);
                }
                if (currentPlayer.GetLatitude() != (float)Convert.ToDouble(data.latitude) || currentPlayer.GetLongitude() != (float)Convert.ToDouble(data.longitude))
                {
                    currentPlayer.SetLatitude((float)Convert.ToDouble(data.latitude));
                    currentPlayer.SetLongitude((float)Convert.ToDouble(data.longitude));
                }
                if (currentPlayer.GetPet().GetPosX() != (float)Convert.ToDouble(data.petPosX) || currentPlayer.GetPet().GetPosY() != (float)Convert.ToDouble(data.petPosY))
                {
                    currentPlayer.GetPet().SetPosX((float)Convert.ToDouble(data.petPosX));
                    currentPlayer.GetPet().SetPosY((float)Convert.ToDouble(data.petPosY));
                }
                if (currentPlayer.GetPet().GetPetName() == "")
                {
                    currentPlayer.GetPet().SetPetName((string)data.petName);
                }
            }

            currentPlayer.GetMapController().SetMapReady(false);
            this.needCreateMap = currentPlayer.CheckAndCreateMap();
            while (!currentPlayer.GetMapController().GetMapReady()) { }

            this.playerPosX = currentPlayer.GetMapController().GetPosX();
            this.playerPosY = currentPlayer.GetMapController().GetPosY();

            this.centerPosX = currentPlayer.GetMapController().GetCenterPosX();
            this.centerPosY = currentPlayer.GetMapController().GetCenterPosY();

            this.tileX = currentPlayer.GetMapController().GetTileX();
            this.tileY = currentPlayer.GetMapController().GetTileY();

            ListMapData mapData = currentPlayer.GetMapController().GetListMapData();

            return mapData;
        }

        public void AddPlayer(string username)
        {
            Player player = listPlayer.Find(x => x.GetPlayerName() == username);
            if (player == null)
            {
                Player newPlayer = new Player(username, 0, 0, "", 0, 0);
                newPlayer.SetIsActive(false);
                listPlayer.Add(newPlayer);
            }else
            {
                player.SetLatitude(0);
                player.SetLongitude(0);
                player.GetPet().SetPosX(0);
                player.GetPet().SetPosY(0);
            }
            
        }

        public int SetPlayerActiveFalse(string username, bool active)
        {
            int result = -1;
      
            Player currentPlayer = listPlayer.Find(x => x.GetPlayerName() == username);
            if (currentPlayer != null)
            {
                currentPlayer.SetIsActive(active);
                currentPlayer.GetPet().SetLastPosX(0f);
                currentPlayer.GetPet().SetLastPosY(0f);
                result = 1;
            }

            return result;
        }

        public int RemovePlayer(dynamic data)
        {
            int result = -1;
            Player currentPlayer = listPlayer.Find(x => x.GetPlayerName() == (string)data.username);
            if (currentPlayer != null)
            {
                listPlayer.Remove(currentPlayer);
                result = 1;
            }

            return result;
        }

        public List<Coordinate> PlayerRouting(dynamic data)
        {
            Player player = listPlayer.Find(x => x.GetPlayerName() == (string)data.username);
            float latitude = (float)Convert.ToDouble(data.latitude);
            float longitude = (float)Convert.ToDouble(data.longitude);

            List<Coordinate> route = player.GetMapController().StartRoute(latitude, longitude, (string)data.destination);

            return route;
        }

        public List<UnityPlayerPetPosition> GetOthers(int tileX, int tileY)
        {
            listPlayerPos.Clear();
            for (int i=0; i<listPlayer.Count; i++)
            {
                Player player = listPlayer.ElementAt<Player>(i);

                if (player.GetMapController().GetTileX() == tileX && player.GetMapController().GetTileY() == tileY)
                {
                    if (player.GetIsActive())
                    {
                        float[] pos = GeoConverter.GeoCoorToMercatorProjection(player.GetLatitude(), player.GetLongitude());

                        UnityPlayerPetPosition playerInList = new UnityPlayerPetPosition();
                        playerInList.playerName = player.GetPlayerName();
                        playerInList.posX = pos[0];
                        playerInList.posY = pos[1];
                        playerInList.petName = player.GetPet().GetPetName();
                        playerInList.petPosX = player.GetPet().GetPosX();
                        playerInList.petPosY = player.GetPet().GetPosY();
                        playerInList.petLastPosX = player.GetPet().GetLastPosX();
                        playerInList.petLastPosY = player.GetPet().GetLastPosY();
                        playerInList.timeStartMove = player.GetPet().GetTimeStartMove();
                        playerInList.petState = player.GetPet().GetPetState();
                        playerInList.ballState = player.GetPet().GetBallState();
                        playerInList.petSpeed = player.GetPet().GetSpeed();

                        listPlayerPos.Add(playerInList);
                    }
                }
            }

            return listPlayerPos;
        }

        public void UpdatePetLocation(dynamic data)
        {
            Player curPlayer = listPlayer.Find(x => x.GetPlayerName() == (string)data.username);
            if (curPlayer != null)
            {
                float lastPosX = float.MinValue;
                float lastPosY = float.MinValue;

                if ((float)data.petLastPosX == 0f && (float)data.petLastPosY == 0f)
                {
                    lastPosX = curPlayer.GetPet().GetPosX();
                    lastPosY = curPlayer.GetPet().GetPosY();
                }else
                {
                    lastPosX = (float)data.petLastPosX + curPlayer.GetMapController().GetCenterPosX();
                    lastPosY = (float)data.petLastPosY + curPlayer.GetMapController().GetCenterPosY();
                }

                if (lastPosX == 0f && lastPosY == 0f)
                {
                    curPlayer.GetPet().SetLastPosX(curPlayer.GetMapController().GetCenterPosX());
                    curPlayer.GetPet().SetLastPosY(curPlayer.GetMapController().GetCenterPosY());
                }else
                {
                    curPlayer.GetPet().SetLastPosX(lastPosX);
                    curPlayer.GetPet().SetLastPosY(lastPosY);
                }

                curPlayer.GetPet().SetTimeStartMove((string)data.timeStartMove);

                curPlayer.GetPet().SetPosX((float)data.petPosX + curPlayer.GetMapController().GetCenterPosX());
                curPlayer.GetPet().SetPosY((float)data.petPosY + curPlayer.GetMapController().GetCenterPosY());

                curPlayer.GetPet().SetPetState((string)data.petState);

                curPlayer.GetPet().SetSpeed((float)data.speed);
            }
        }

        public float[] UpdateBallState(dynamic data)
        {
            float[] ballPos = new float[] { (float)data.ballPosX, (float)data.ballPosZ };

            Player curPlayer = listPlayer.Find(x => x.GetPlayerName() == (string)data.username);
            if (curPlayer != null)
            {
                curPlayer.GetPet().SetBallState((string)data.ballState);
                if ((string)data.ballState != "throw")
                {
                    ballPos[0] += curPlayer.GetMapController().GetCenterPosX();
                    ballPos[1] += curPlayer.GetMapController().GetCenterPosY();
                }
            }

            return ballPos;
        }

        public float GetPlayerPosX()
        {
            return this.playerPosX;
        }

        public float GetPlayerPosY()
        {
            return this.playerPosY;
        }

        public bool GetNeedCreateMap()
        {
            return this.needCreateMap;
        }

        public float GetCenterPosX()
        {
            return this.centerPosX;
        }

        public float GetCenterPosY()
        {
            return this.centerPosY;
        }

        public int GetTileX()
        {
            return this.tileX;
        }

        public int GetTileY()
        {
            return this.tileY;
        }
    }
}
