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
        var msg = AmqpController.amqpControl.msg;
        if (msg != null)
        {
            //Debug.Log(msg);
            string responseId = (string)msg["id"];
            if (id == responseId)
            {
                string type = (string)msg["type"];
                if (type == "returntitle")
                {
                    int result = (int)msg["result"];
                    if (result == 1)
                    {
                        AmqpController.amqpControl.msg = null;
                        DataControllerScript.dataController.isLogin = false;
                        SceneManager.LoadScene("MainMenuScene");
                    }
                }
            }

            msg = null;
        }
            
	}

    public void returnTitle()
    {
        //Debug.Log("masuk return title");

        ReturnTitleJson request = new ReturnTitleJson();
        request.id = id;
        request.type = "returntitle";

        request.username = DataControllerScript.dataController.username;
        request.energy = (int)DataControllerScript.dataController.energy;
        request.hunger = (int)DataControllerScript.dataController.hunger;
        request.fun = (int)DataControllerScript.dataController.fun;
        request.hygiene = (int)DataControllerScript.dataController.hygiene;
        request.environment = (int)DataControllerScript.dataController.environment;
        string jsonRequest = JsonUtility.ToJson(request);

        AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, jsonRequest);
    }

    [Serializable]
    class ReturnTitleJson
    {
        public string id;
        public string type;

        public string username;
        public int energy;
        public int hunger;
        public int fun;
        public int hygiene;
        public int environment;
    }
}
