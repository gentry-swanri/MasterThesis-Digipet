using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using CymaticLabs.Unity3D.Amqp;

public class ReturnTitleScript : MonoBehaviour {

    public Button title;
    public string id;

    // Use this for initialization
    void Start () {
        id = Guid.NewGuid().ToString();

        title = this.GetComponent<Button>();
        title.onClick.AddListener(returnTitle);
	}
	
	// Update is called once per frame
	void Update () {
            
	}

    void ProcessReturnTitle(AmqpExchangeReceivedMessage received)
    {
        var receivedJson = System.Text.Encoding.UTF8.GetString(received.Message.Body);
        var msg = CymaticLabs.Unity3D.Amqp.SimpleJSON.JSON.Parse(receivedJson);
        if (msg != null)
        {
            string responseId = (string)msg["id"];
            if (id == responseId)
            {
                string type = (string)msg["type"];
                if (type == "returntitle")
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

    public void returnTitle()
    {
        AmqpController.amqpControl.exchangeSubscription.Handler = ProcessReturnTitle;

        ReturnTitleJson request = new ReturnTitleJson();
        request.id = id;
        request.type = "returntitle";

        request.username = PlayerPrefs.GetString("username");
        request.rest = PlayerPrefs.GetInt("Sleep");
        request.energy = PlayerPrefs.GetInt("Food");
        request.agility = PlayerPrefs.GetInt("Walk");
        request.stress = PlayerPrefs.GetInt("Fun");
        request.heart = PlayerPrefs.GetInt("Health");
        request.money = PlayerPrefs.GetInt("Bones");
        request.xp = PlayerPrefs.GetInt("XP");

        string jsonRequest = JsonUtility.ToJson(request);
        Debug.Log(jsonRequest);

        AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, jsonRequest);
        
    }

    [Serializable]
    class ReturnTitleJson
    {
        public string id;
        public string type;

        public string username;
        public int rest;
        public int energy;
        public int agility;
        public int stress;
        public int heart;
        public int money;
        public int xp;
    }
}
