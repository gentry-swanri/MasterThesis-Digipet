using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using CymaticLabs.Unity3D.Amqp;

public class CreateAccountScript : MonoBehaviour {

    string id;
    Button createButton;
    Text warningText;

	// Use this for initialization
	void Start () {
        id = Guid.NewGuid().ToString();

        createButton = this.GetComponent<Button>();
        createButton.onClick.AddListener(CreateNewAccount);

        warningText = GameObject.Find("WarningText").GetComponent<Text>();
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
                if (type == "newaccount")
                {
                    int result = (int)msg["result"];
                    if (result == -2)
                    {
                        warningText.text = "username already exist";
                    }else if (result == -1)
                    {
                        warningText.text = "Server Error";
                    }else
                    {
                        AmqpController.amqpControl.msg = null;
                        SceneManager.LoadScene("MainMenuScene");
                    }
                }
            }
        }
	}

    void CreateNewAccount()
    {
        InputField firstNameField = GameObject.Find("FirstNameInputField").GetComponent<InputField>();
        InputField lastNameFiled = GameObject.Find("LastNameInputField").GetComponent<InputField>();
        InputField emailField = GameObject.Find("EmailInputField").GetComponent<InputField>();
        InputField usernameField = GameObject.Find("UsernameInputField").GetComponent<InputField>();
        InputField petNameField = GameObject.Find("PetNameInputField").GetComponent<InputField>();
        InputField passwordField = GameObject.Find("PasswordInputField").GetComponent<InputField>();

        string firstName = firstNameField.text;
        string lastName = lastNameFiled.text;
        string email = emailField.text;
        string username = usernameField.text;
        string petName = petNameField.text;
        string password = passwordField.text;

        if (firstName == "")
        {
            warningText.text = "first name cannot be empty";
        }

        if (lastName == "")
        {
            warningText.text = "last name cannot be empty";
        }

        if (email == "")
        {
            warningText.text = "email cannot be empty";
        }

        if (username == "")
        {
            warningText.text = "username cannot be empty";
        }

        if (petName == "")
        {
            warningText.text = "pet name cannot be empty";
        }

        if (password == "")
        {
            warningText.text = "password cannot be empty";
        }

        if (firstName != "" && lastName != "" && email != "" && username != "" && petName != "" && password != "")
        {
            RequestJson request = new RequestJson();
            request.id = id;
            request.type = "newaccount";
            request.firstName = firstName;
            request.lastName = lastName;
            request.email = email;
            request.username = username;
            request.petName = petName;
            request.password = password;

            string requestJson = JsonUtility.ToJson(request);

            //Debug.Log(requestJson);

            AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);
        }
    }

    [Serializable]
    public class RequestJson
    {
        public string id;
        public string type;
        public string firstName;
        public string lastName;
        public string email;
        public string username;
        public string petName;
        public string password;
    }
}
