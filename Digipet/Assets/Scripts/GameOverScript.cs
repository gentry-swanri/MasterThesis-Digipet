using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using CymaticLabs.Unity3D.Amqp;

public class GameOverScript : MonoBehaviour {

    Button restartBut;
    string id;

	// Use this for initialization
	void Start () {
        id = Guid.NewGuid().ToString();
        restartBut = this.GetComponent<Button>();
        restartBut.onClick.AddListener(restartAction);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ProcessRestart(AmqpExchangeReceivedMessage received)
    {
        var receivedJson = System.Text.Encoding.UTF8.GetString(received.Message.Body);
        var msg = CymaticLabs.Unity3D.Amqp.SimpleJSON.JSON.Parse(receivedJson);

        //var msg = AmqpController.amqpControl.msg;
        if (msg != null)
        {
            string msgId = (string)msg["id"];
            if (msgId == id)
            {
                string type = (string)msg["type"];
                if (type == "restart")
                {
                    int result = (int)msg["result"];
                    if (result == 1)
                    {
                        SceneManager.LoadScene("MainMenuScene");
                    }
                }
            }
        }
    }

    void restartAction()
    {
        AmqpController.amqpControl.exchangeSubscription.Handler = ProcessRestart;

        RestartRequest json = new RestartRequest();
        json.id = id;
        json.type = "restart";
        json.username = PlayerPrefs.GetString("username");

        string request = JsonUtility.ToJson(json);

        AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, request);
    }

    [Serializable]
    class RestartRequest
    {
        public string id;
        public string type;
        public string username;
    }
}
