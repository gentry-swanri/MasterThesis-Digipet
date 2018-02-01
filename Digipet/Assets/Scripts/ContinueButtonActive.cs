using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ContinueButtonActive : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        if (!PlayerPrefs.HasKey("Health") ||
            (PlayerPrefs.HasKey("Health") && PlayerPrefs.GetInt("Health") == 0))
        {
            //Debug.Log("Health is " + PlayerPrefs.GetInt("Health"));
            GetComponent<Button>().interactable = false;
        }
	}
	
	
}
