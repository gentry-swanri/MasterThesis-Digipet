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
            }

            currentPlayer.GetMapController().SetMapReady(false);
            this.needCreateMap = currentPlayer.CheckAndCreateMap();
            while (!currentPlayer.GetMapController().GetMapReady()) { }

            this.playerPosX = currentPlayer.GetMapController().GetPosX();
            this.playerPosY = currentPlayer.GetMapController().GetPosY();

            ListMapData mapData = currentPlayer.GetMapController().GetListMapData();

            return mapData;
        }

        public int SetPlayerActive(dynamic data)
        {
            int result = -1;
            Player currentPlayer = listPlayer.Find(x => x.GetPlayerName() == (string)data.username);
            if (currentPlayer != null)
            {
                currentPlayer.SetIsActive(false);
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
            //Player player = listPlayer.Find(x => x.GetPlayerId() == (string)data.id);
            Player player = listPlayer.Find(x => x.GetPlayerName() == (string)data.username);
            float latitude = (float)Convert.ToDouble(data.latitude);
            float longitude = (float)Convert.ToDouble(data.longitude);

            List<Coordinate> route = player.GetMapController().StartRoute(latitude, longitude, (string)data.destination);

            return route;
        }

        public List<UnityPlayerPetPosition> GetOthersInRange(dynamic data, float range)
        {
            listPlayerPos.Clear();

            Player curPlayer = listPlayer.Find(x => x.GetPlayerName() == (string)data.username);
            float[] curPlayerPos = GeoConverter.GeoCoorToMercatorProjection(curPlayer.GetLatitude(), curPlayer.GetLongitude());

            for (int i=0; i<listPlayer.Count; i++)
            {
                Player player = listPlayer.ElementAt<Player>(i);
                if (player.GetPlayerName() != (string)data.username)
                {
                    float[] pos = GeoConverter.GeoCoorToMercatorProjection(player.GetLatitude(), player.GetLongitude());
                    if (pos[0] < curPlayerPos[0] + range && pos[1] < curPlayerPos[1] + range)
                    {
                        UnityPlayerPetPosition playerInRange = new UnityPlayerPetPosition();
                        //playerInRange.playerId = player.GetPlayerId();
                        playerInRange.playerName = player.GetPlayerName();
                        playerInRange.posX = pos[0] - curPlayerPos[0];
                        playerInRange.posY = pos[1] - curPlayerPos[1];
                        playerInRange.petName = player.GetPet().GetPetName();
                        playerInRange.petPosX = (player.GetPet().GetPosX() + pos[0]) - curPlayerPos[0];
                        playerInRange.petPosY = (player.GetPet().GetPosY() + pos[1]) - curPlayerPos[1];

                        listPlayerPos.Add(playerInRange);
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
                curPlayer.GetPet().SetPosX((float)data.petPosX);
                curPlayer.GetPet().SetPosY((float)data.petPosY);
            }
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
    }
}
