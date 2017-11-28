using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using CymaticLabs.Unity3D.Amqp;

public class PauseScript : MonoBehaviour {

    private Button pauseButton;
    public string id;

	// Use this for initialization
	void Start () {
        id = Guid.NewGuid().ToString();

        pauseButton = this.GetComponent<Button>();
        pauseButton.onClick.AddListener(PauseGame);
	}
	
	// Update is called once per frame
	void Update () {
        var msg = AmqpController.amqpControl.msg;
 
        if (msg != null)
        {
            string responseId = (string)msg["id"];
            if (responseId == id)
            {
                string type = (string)msg["type"];
                if (type == "pause")
                {
                    if ((int)msg["result"] == 1)
                    {
                        GameObject plane = GameObject.Find("Plane");
                        WebCamTexture webcamTexture = (WebCamTexture)plane.GetComponent<MeshRenderer>().material.mainTexture;
                        webcamTexture.Stop();

                        AmqpController.amqpControl.msg = null;

                        if (Input.location.status == LocationServiceStatus.Running)
                        {
                            Input.location.Stop();
                        }

                        SceneManager.LoadScene("PauseMenuScene");
                    }
                }
            }
        }
	}

    void PauseGame()
    {
        PauseRequest request = new PauseRequest();
        request.id = id;
        request.type = "pause";
        request.username = DataControllerScript.dataController.username;
        string jsonRequest = JsonUtility.ToJson(request);

        AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, jsonRequest);
    }

    [Serializable]
    class PauseRequest
    {
        public string id;
        public string type;
        public string username;
    }
}
