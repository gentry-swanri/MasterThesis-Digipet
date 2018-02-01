using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using CymaticLabs.Unity3D.Amqp;

public class ChangeScript : MonoBehaviour {

    Button changePassButton;
    Text warning;
    string id;

    // Use this for initialization
    void Start () {
        id = Guid.NewGuid().ToString();

        warning = GameObject.Find("WarningText").GetComponent<Text>();

        changePassButton = this.GetComponent<Button>();
        changePassButton.onClick.AddListener(ChangeAction);
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void ProcessChangePass(AmqpExchangeReceivedMessage received)
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
                if (type == "changepassword")
                {
                    int result = (int)msg["result"];
                    if (result == 1)
                    {
                        warning.color = Color.blue;
                        warning.text = "Password changed succesfully";
                        //AmqpController.amqpControl.msg = null;
                    }
                    else
                    {
                        warning.text = "Error Occured";
                    }
                }
            }
        }
    }

    public void ChangeAction()
    {
        warning.text = "";

        InputField inputOld = GameObject.Find("OldPassInputField").GetComponent<InputField>();
        InputField inputNew = GameObject.Find("NewPassInputField").GetComponent<InputField>();
        InputField inputRetype = GameObject.Find("RetypePassInputField").GetComponent<InputField>();

        string old = inputOld.text;
        string newPass = inputNew.text;
        string retype = inputRetype.text;

        if (old == "")
        {
            warning.text = "Please fill in old password";
        }

        if (newPass == "")
        {
            warning.text = "Please fill in new password";
        }

        if (retype == "")
        {
            warning.text = "Please retype new password";
        }

        if (old != "" && newPass != "" && retype != "")
        {
            if (newPass == retype)
            {
                AmqpController.amqpControl.exchangeSubscription.Handler = ProcessChangePass;

                ChangePassJson json = new ChangePassJson();
                json.id = id;
                json.type = "changepassword";
                json.username = PlayerPrefs.GetString("username");
                json.oldPass = old;
                json.newPass = newPass;

                string request = JsonUtility.ToJson(json);

                AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, request);
            }else
            {
                warning.text = "Retype password is different from new password";
            }
        }
    }

    [Serializable]
    public class ChangePassJson
    {
        public string id;
        public string type;
        public string username;
        public string oldPass;
        public string newPass;
    }
}
