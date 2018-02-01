using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CymaticLabs.Unity3D.Amqp;

public class AmqpController : MonoBehaviour {

    public static AmqpController amqpControl;

    public bool serverConnected;

    // connection properties section
    public string requestExchangeName;
    public string requestRoutingKey;

    public string responseExchangeName;
    public string responseRoutingKey;
    public AmqpExchangeTypes responseExchangeType;

    public AmqpExchangeSubscription exchangeSubscription;

    // response message
    //public CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode msg;

    void Awake()
    {
        if (amqpControl == null)
        {
            DontDestroyOnLoad(this.gameObject);
            amqpControl = this;
        }else if (amqpControl != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        serverConnected = false;

        // intialize response message with null
        //msg = null;

        // initialize connection properties
        requestExchangeName = "DigipetRequestExchange";
        requestRoutingKey = "DigipetRequestRoutingKey";

        responseExchangeName = "DigipetResponseExchange";
        responseRoutingKey = "DigipetResponseRoutingKey";
        responseExchangeType = AmqpExchangeTypes.Direct;
        //responseExchangeType = AmqpExchangeTypes.Fanout;

        // connect to rabbitmq server

        //AmqpClient.Instance.ConnectOnStart = true;
        AmqpClient.Instance.Connection = "ITB";
        //AmqpClient.Instance.Connection = "localhost";
        AmqpClient.Connect();

        // handle event after connected to rabbitmq server
        AmqpClient.Instance.OnConnected.AddListener(HandleConnected);
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    // subscribe to rabbitmq exchange if connection is successful
    void HandleConnected(AmqpClient clientParam)
    {
        //Debug.Log("Connect");
        // subscribe to exchange for listening response's purpose
        exchangeSubscription = new AmqpExchangeSubscription(responseExchangeName, responseExchangeType, responseRoutingKey, HandleExchangeMessageReceived);
        AmqpClient.Subscribe(exchangeSubscription);

        serverConnected = true;

        /*
        GameObject a = GameObject.Find("Test");
        Text b = a.GetComponent<Text>();
        b.text = "CONNECTED";

        Debug.Log("Connect");
        */

        // testing publish message to server
        // AmqpClient.Publish(this.requestExchangeName, this.requestRoutingKey, "test");
    }

    void HandleExchangeMessageReceived(AmqpExchangeReceivedMessage received)
    {
        var receivedJson = System.Text.Encoding.UTF8.GetString(received.Message.Body);
        Debug.Log("JSON Murni = "+receivedJson);
        var msg = CymaticLabs.Unity3D.Amqp.SimpleJSON.JSON.Parse(receivedJson);
        Debug.Log("JSON Decode = "+msg);
    }
}
