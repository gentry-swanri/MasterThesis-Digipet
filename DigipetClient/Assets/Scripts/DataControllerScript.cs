using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DataControllerScript : MonoBehaviour {

    public static DataControllerScript dataController;

    public string username;
    public string petName;
    //public DateTime firstTime;
    //public DateTime lastSave;
    public int age;
    public float energy;
    public float hunger;
    public float fun;
    public float hygiene;
    public float environment;

    public bool isLogin;

    void Awake()
    {
        if (dataController == null)
        {
            DontDestroyOnLoad(this.gameObject);
            dataController = this;
        }else if (dataController != this)
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        username = "Player";
        petName = "PlayerPet";
        //firstTime = DateTime.MinValue;
        //lastSave = DateTime.MinValue;
        age = 0;
        energy = 100;
        hunger = 0;
        fun = 100;
        hygiene = 100;
        environment = 100;

        isLogin = false;
    }

    public void AddEnergy(float energyValue)
    {
        energy += energyValue;
        if (energy > 100)
        {
            energy = 100;
        }
    }

    public void ReduceEnergy(float energyValue)
    {
        energy -= energyValue;
        if (energy < 0)
        {
            energy = 0;
        }
    }

    public void AddHunger(float hungerValue)
    {
        hunger += hungerValue;
        if (hunger > 100)
        {
            hunger = 100;
        }
    }

    public void ReduceHunger(float hungerValue)
    {
        hunger -= hungerValue;
        if (hunger < 0)
        {
            hunger = 0;
        }
    }

    public void AddFun(float funValue)
    {
        fun += funValue;
        if (fun > 100)
        {
            fun = 100;
        }
    }

    public void ReduceFun(float funValue)
    {
        fun -= funValue;
        if (fun < 0)
        {
            fun = 0;
        }
    }

    public void AddHygiene(float hygieneValue)
    {
        hygiene += hygieneValue;
        if (hygiene > 100)
        {
            hygiene = 100;
        }
    }

    public void ReduceHygiene(float hygieneValue)
    {
        hygiene -= hygieneValue;
        if (hygiene < 0)
        {
            hygiene = 0;
        }
    }

    public void AddEnvironment(float environmentValue)
    {
        environment += environmentValue;
        if (environment > 100)
        {
            environment = 100;
        }
    }

    public void ReduceEnvironment(float environmentValue)
    {
        environment -= environmentValue;
        if (environment < 0)
        {
            environment = 0;
        }
    }

    /*
    public int CalculateOld()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan rangeTime = currentTime.Subtract(firstTime);
        return rangeTime.Days / 356;
    }

    public void UpdateData()
    {
        int dayHour = 0;

        DateTime currentTime = DateTime.Now;
        TimeSpan rangeTime = currentTime.Subtract(lastSave);

        if (rangeTime.Days != 0)
        {
            dayHour = rangeTime.Days * 24;
        }

        int totalHour = dayHour + rangeTime.Hours;

        this.ReduceEnergy(totalHour * 5);
        this.ReduceHunger(totalHour * 5);
        this.ReduceFun(totalHour * 5);
        this.ReduceHygiene(totalHour * 5);
        this.ReduceEnvironment(totalHour * 5);
    }

    public void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/petdata.dat", FileMode.Open);

        PetData petData = new PetData();
        petData.username = username;
        petData.petName = petName;

        //petData.firstTime = firstTime;
        //petData.lastSave = DateTime.Now;

        petData.firstTimeString = firstTime.ToString();
        petData.lastSaveString = lastSave.ToString();

        petData.age = CalculateOld();
        petData.energy = energy;
        petData.hunger = hunger;
        petData.fun = fun;
        petData.hygiene = hygiene;
        petData.environment = environment;

        bf.Serialize(file, petData);
        file.Close();
    }

    public bool LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/petdata.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/petdata.dat", FileMode.Open);
            PetData petData = (PetData)bf.Deserialize(file);
            file.Close();

            username = petData.username;
            petName = petData.petName;

            //firstTime = petData.firstTime;
            //lastSave = petData.lastSave;

            firstTime = DateTime.Parse(petData.firstTimeString);
            lastSave = DateTime.Parse(petData.lastSaveString);

            age = petData.age;
            energy = petData.energy;
            hunger = petData.hunger;
            fun = petData.fun;
            hygiene = petData.hygiene;
            environment = petData.environment;

            UpdateData();

            return true;
        }else
        {
            return false;
        }
    }
    */

}

/*
[Serializable]
class PetData
{
    public string username;
    public string petName;

    //public DateTime firstTime;
    //public DateTime lastSave;

    public string firstTimeString;
    public string lastSaveString;

    public int age;
    public float energy;
    public float hunger;
    public float fun;
    public float hygiene;
    public float environment;
}
*/