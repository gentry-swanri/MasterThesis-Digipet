using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScript : MonoBehaviour {

    //float time;
    //float timeMax;

	// Use this for initialization
	void Start () {
        //time = 0.0f;
        //timeMax = 2.0f;
	}
	
	// Update is called once per frame
	void Update () {
        //time += Time.deltaTime;

        if (AmqpController.amqpControl != null)
        {
            if (AmqpController.amqpControl.serverConnected)
            {
                SceneManager.LoadScene("MainMenuScene");
            }
        }
	}
}
