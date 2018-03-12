using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyrightScript : MonoBehaviour {

    private Text copyrightText;

	// Use this for initialization
	void Start () {
        copyrightText = this.GetComponent<Text>();
        string copyrightSymbol = "\u00A9";
        copyrightText.text = copyrightSymbol + " OpenStreetMap contributors";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
