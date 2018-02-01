using UnityEngine;
using System.Collections;

// this script will be attached to each of the food items in the shop

public class ShopItem : MonoBehaviour 
{
    public FoodItem Item;
    public int Price;
    public int Quantity;

    public void Buy()
    {
        Debug.Log("Attempt to buy  " + Item.ItemName);
        if (SaveManager.Instance.Bones >= Price)
        {
            SaveManager.Instance.Bones -= Price;
            FoodManager.Instance.FoodCollection[Item.ItemName] += Quantity;
        }
    }
}
