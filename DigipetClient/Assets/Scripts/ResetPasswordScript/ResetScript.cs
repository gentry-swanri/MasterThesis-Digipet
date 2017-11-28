using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using CymaticLabs.Unity3D.Amqp;

public class ResetScript : MonoBehaviour {

    Button resetBut;
    Text warning;
    string id;

	// Use this for initialization
	void Start () {
        id = Guid.NewGuid().ToString();

        warning = GameObject.Find("WarningText").GetComponent<Text>();

        resetBut = this.GetComponent<Button>();
        resetBut.onClick.AddListener(ResetAction);
	}
	
	// Update is called once per frame
	void Update () {
        var msg = AmqpController.amqpControl.msg;
        if (msg != null)
        {
            string msgId = (string)msg["id"];
            if (msgId == id)
            {
                string type = (string)msg["type"];
                if (type == "resetpassword")
                {
                    int result = (int)msg["result"];
                    if (result == 1)
                    {
                        warning.color = Color.blue;
                        warning.text = "New password has been sent";
                        AmqpController.amqpControl.msg = null;
                    }else
                    {
                        warning.text = "Email not found";
                    }
                }
            }
        }
	}

    public void ResetAction()
    {
        warning.text = "";

        InputField input = GameObject.Find("EmailInput").GetComponent<InputField>();
        string email = input.text;

        if (email == "")
        {
            warning.text = "Please fill in your email";
        }else
        {
            ResetRequestJson request = new ResetRequestJson();
            request.id = id;
            request.type = "resetpassword";
            request.email = email;

            string json = JsonUtility.ToJson(request);
            //Debug.Log(json);
            AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, json);
        }

    }

    [Serializable]
    public class ResetRequestJson
    {
        public string id;
        public string type;
        public string email;
    }
}
