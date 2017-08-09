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
        private List<UnityPlayerPosition> listPlayerPos;

        public PlayerManagement()
        {
            listPlayer = new List<Player>();
            listPlayerPos = new List<UnityPlayerPosition>();
        }

        public dynamic AcquireMapData(dynamic data)
        {
            Player currentPlayer;
         
            bool registered = listPlayer.Exists(x => x.GetPlayerName() == (string)data.playerName);
            if (!registered)
            {
                currentPlayer = new Player(data.id.ToString(), data.playerName.ToString(), (float)Convert.ToDouble(data.latitude), (float)Convert.ToDouble(data.longitude), data.petName.ToString(), (float)Convert.ToDouble(data.petPosX), (float)Convert.ToDouble(data.petPosY));
                listPlayer.Add(currentPlayer);
            }
            else
            {
                currentPlayer = listPlayer.Find(x => x.GetPlayerId() == (string)data.id);
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

            dynamic mapData = currentPlayer.GetMapController().GetCompleteMapData();

            return mapData;
        }

        public void SetPlayerActive(dynamic data)
        {
            Player currentPlayer = listPlayer.Find(x => x.GetPlayerName() == (string)data.playerName);
            if (currentPlayer != null)
            {
                currentPlayer.SetIsActive(false);
            }
        }

        public List<Coordinate> PlayerRouting(dynamic data)
        {
            Player player = listPlayer.Find(x => x.GetPlayerId() == (string)data.id);
            float latitude = (float)Convert.ToDouble(data.latitude);
            float longitude = (float)Convert.ToDouble(data.longitude);

            List<Coordinate> route = player.GetMapController().StartRoute(latitude, longitude, (string)data.destination);

            return route;
        }

        public List<UnityPlayerPosition> GetOthersInRange(dynamic data, float range)
        {
            Player curPlayer = listPlayer.Find(x => x.GetPlayerName() == (string)data.playerName);
            float[] curPlayerPos = GeoConverter.GeoCoorToMercatorProjection(curPlayer.GetLatitude(), curPlayer.GetLongitude());

            listPlayerPos.Clear();
            for (int i=0; i<listPlayer.Count; i++)
            {
                Player player = listPlayer.ElementAt<Player>(i);
                if (player.GetPlayerId() != (string)data.id)
                {
                    float[] pos = GeoConverter.GeoCoorToMercatorProjection(curPlayer.GetLatitude(), curPlayer.GetLongitude());
                    if (pos[0] < curPlayerPos[0] + range && pos[1] < curPlayerPos[1] + range)
                    {
                        UnityPlayerPosition playerInRange = new UnityPlayerPosition();
                        playerInRange.playerId = player.GetPlayerId();
                        playerInRange.playerName = player.GetPlayerName();
                        playerInRange.posX = pos[0];
                        playerInRange.posY = pos[1];

                        listPlayerPos.Add(playerInRange);
                    }
                }
            }

            return listPlayerPos;
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
