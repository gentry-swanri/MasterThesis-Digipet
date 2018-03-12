using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (AmqpController.amqpControl != null)
        {
            if (AmqpController.amqpControl.serverConnected)
            {
                SceneManager.LoadScene("MainMenuScene");
            }
        }
	}
}
