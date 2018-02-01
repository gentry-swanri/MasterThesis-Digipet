using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// this script will manage and save data about all the food that our character has

public class FoodManager : MonoBehaviour 
{
    public Dictionary<string,int> FoodCollection = new Dictionary<string, int>();
    public FoodItem[] AllFoodItems;
    public static FoodManager Instance; 
    public Care FoodCare; 

    void Awake()
    {
        Instance = this;
        // populate food collection dictionary
        FoodCollection.Clear();

        foreach (FoodItem f in AllFoodItems)
        {
            FoodCollection.Add(f.ItemName, 2);
        }

        LoadFood();
    }

    void Start()
    {
        
    }

    public void ConsumeFoodItem(FoodItem item)
    {
        if (FoodCollection[item.ItemName] >0)
        {
            FoodCollection[item.ItemName]--;
            FoodCare.CarePoints += item.PointsAddedToCare;
        }
    }

    public void SaveFood()
    {
        foreach (FoodItem f in AllFoodItems)
            PlayerPrefs.SetInt(f.ItemName, FoodCollection[f.ItemName]);
    }

    private void LoadFood()
    {
        foreach (FoodItem f in AllFoodItems)
        {
            if (PlayerPrefs.HasKey(f.ItemName))
                FoodCollection[f.ItemName] = PlayerPrefs.GetInt(f.ItemName);
        }
    }

    void OnApplicationQuit()
    {
        SaveFood();
    }
}

[System.Serializable]
public struct FoodItem
{
    public string ItemName;
    public int PointsAddedToCare;
}
