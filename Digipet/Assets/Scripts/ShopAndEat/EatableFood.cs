using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// this represents a food item that we already have 
// we can see it in the Eat menu in our Game

public class EatableFood : MonoBehaviour {

    public FoodItem Item;
    public Text QuantityText;

    void Start()
    {
        UpdateText();   
    }

    public void Eat()
    {
        Debug.Log("Attempt to eat  " + Item.ItemName);
        FoodManager.Instance.ConsumeFoodItem(Item);
        UpdateText();
    }

    public void UpdateText()
    {
        QuantityText.text = FoodManager.Instance.FoodCollection[Item.ItemName].ToString();
    }
}
