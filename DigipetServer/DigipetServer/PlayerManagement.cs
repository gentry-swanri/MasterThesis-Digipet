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

        public PlayerManagement()
        {
            listPlayer = new List<Player>();
        }

        public dynamic AcquireMapData(dynamic data)
        {
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
