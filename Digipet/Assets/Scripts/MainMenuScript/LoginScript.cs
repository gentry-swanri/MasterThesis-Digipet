using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CymaticLabs.Unity3D.Amqp;

public class LoginScript : MonoBehaviour {

    Button login;
    Text warning;
    string id;

    // Use this for initialization
    void Start () {
        id = Guid.NewGuid().ToString();

        warning = GameObject.Find("WarningText").GetComponent<Text>();

        login = this.GetComponent<Button>();
        login.onClick.AddListener(loginGame);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    void ProcessLogin(AmqpExchangeReceivedMessage received)
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
                if (type == "login")
                {
                    int count = (int)msg["count"];
                    if (count == 1)
                    {
                        LoadStatus(msg);
                        //AmqpController.amqpControl.msg = null;
                        Time.timeScale = 1;
                        SceneManager.LoadScene("MainScene");
                    }
                    else
                    {
                        warning.text = "username or password not found";
                    }
                }
            }
        }
    }

    void loginGame()
    {
        
        warning.text = "";

        InputField usernameField = GameObject.Find("UsernameInputField").GetComponent<InputField>();
        InputField passwordField = GameObject.Find("PasswordInputField").GetComponent<InputField>();

        string username = usernameField.text;
        string password = passwordField.text;

        if (username == "")
        {
            warning.text = "username cannot be empty";
        }

        if (password == "")
        {
            warning.text = "password cannot be empty";
        }

        //Debug.Log(username+" ----- "+password);
        if (username != "" && password != "") {
            AmqpController.amqpControl.exchangeSubscription.Handler = ProcessLogin;

            LoginRequestJson loginRequest = new LoginRequestJson();
            loginRequest.id = id;
            loginRequest.type = "login";
            loginRequest.username = username;
            loginRequest.password = password;

            string request = JsonUtility.ToJson(loginRequest);
            AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, request);
        }
        
    }

    void LoadStatus(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode msg)
    {
        /*
        DataControllerScript.dataController.username = (string)msg["username"];
        DataControllerScript.dataController.heart = (int)msg["data"][0];
        DataControllerScript.dataController.money = (int)msg["data"][1];
        DataControllerScript.dataController.xp = (int)msg["data"][2];
        DataControllerScript.dataController.rest = (int)msg["data"][3];
        DataControllerScript.dataController.energy = (int)msg["data"][4];
        DataControllerScript.dataController.agility = (int)msg["data"][5];
        DataControllerScript.dataController.stress = (int)msg["data"][6];
        DataControllerScript.dataController.petName = (string)msg["data"][7];
        */

        PlayerPrefs.SetString("username", (string)msg["username"]);
        PlayerPrefs.SetInt("Health", (int)msg["data"][0]);
        PlayerPrefs.SetInt("Bones", (int)msg["data"][1]);
        PlayerPrefs.SetInt("XP", (int)msg["data"][2]);
        PlayerPrefs.SetInt("Sleep", (int)msg["data"][3]);
        PlayerPrefs.SetInt("Food", (int)msg["data"][4]);
        PlayerPrefs.SetInt("Walk", (int)msg["data"][5]);
        PlayerPrefs.SetInt("Fun", (int)msg["data"][6]);
        PlayerPrefs.SetString("petName", (string)msg["data"][7]);

        //DataControllerScript.dataController.isLogin = true;
    }

    [Serializable]
    public class LoginRequestJson
    {
        public string id;
        public string type;
        public string username;
        public string password;
    }
}
