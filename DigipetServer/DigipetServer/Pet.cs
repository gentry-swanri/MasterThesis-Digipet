using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigipetServer
{
    class Pet
    {
        private string petName;
        private float posX;
        private float posY;
        private float lastPosX;
        private float lastPosY;
        private string timeStartMove;
        private string petState;

        public Pet(string petName, float posX, float posY)
        {
            this.petName = petName;
            this.posX = posX;
            this.posY = posY;
            this.lastPosX = 0f;
            this.lastPosY = 0f;
            this.timeStartMove = "";
            this.petState = "";
        }

        public void SetPetName(string petName)
        {
            this.petName = petName;
        }

        public string GetPetName()
        {
            return this.petName;
        }

        public void SetPosX(float posX)
        {
            this.posX = posX;
        }

        public float GetPosX()
        {
            return this.posX;
        }

        public void SetPosY(float posY)
        {
            this.posY = posY;
        }

        public float GetPosY()
        {
            return this.posY;
        }

        public void SetLastPosX(float lastPosX)
        {
            this.lastPosX = lastPosX;
        }

        public float GetLastPosX()
        {
            return this.lastPosX;
        }

        public void SetLastPosY(float lastPosY)
        {
            this.lastPosY = lastPosY;
        }

        public float GetLastPosY()
        {
            return this.lastPosY;
        }

        public void SetTimeStartMove(string timeStartMove)
        {
            this.timeStartMove = timeStartMove;
        }

        public string GetTimeStartMove()
        {
            return this.timeStartMove;
        }

        public void SetPetState(string petState)
        {
            this.petState = petState;
        }

        public string GetPetState()
        {
            return this.petState;
        }
    }
}
