using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetDataScript : MonoBehaviour {

    Button resetData;

	// Use this for initialization
	void Start () {
        resetData = this.GetComponent<Button>();
        resetData.onClick.AddListener(ResetDataOnClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ResetDataOnClick()
    {
        InputField ipUsername = GameObject.Find("UsernameInputField").GetComponent<InputField>();
        ipUsername.text = "";
        InputField ipPetname = GameObject.Find("PetNameInputField").GetComponent<InputField>();
        ipPetname.text = "";
    }
}
