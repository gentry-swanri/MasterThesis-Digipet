using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeTextScript : MonoBehaviour {

    private Text timeText;
    private DateTime datetime;

	// Use this for initialization
	void Start () {
        timeText = this.GetComponent<Text>();
        timeText.text = "00 : 00 AM";

        datetime = DateTime.Now;
        timeText.text = datetime.ToShortTimeString();
    }
	
	// Update is called once per frame
	void Update () {
        datetime = DateTime.Now;
        if (timeText.text != datetime.ToShortTimeString())
        {
            timeText.text = datetime.ToShortTimeString();
        }
	}
}
