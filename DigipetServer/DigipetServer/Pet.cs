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

        public Pet(string petName, float posX, float posY)
        {
            this.petName = petName;
            this.posX = posX;
            this.posY = posY;
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
    }
}
