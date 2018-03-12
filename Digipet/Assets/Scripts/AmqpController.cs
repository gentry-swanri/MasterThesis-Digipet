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

        // initialize connection properties
        requestExchangeName = "DigipetRequestExchange";
        requestRoutingKey = "DigipetRequestRoutingKey";

        responseExchangeName = "DigipetResponseExchange";
        responseRoutingKey = "DigipetResponseRoutingKey";
        responseExchangeType = AmqpExchangeTypes.Direct;

        // connect to rabbitmq server
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
        // subscribe to exchange for listening response's purpose
        exchangeSubscription = new AmqpExchangeSubscription(responseExchangeName, responseExchangeType, responseRoutingKey, HandleExchangeMessageReceived);
        AmqpClient.Subscribe(exchangeSubscription);

        serverConnected = true;
    }

    void HandleExchangeMessageReceived(AmqpExchangeReceivedMessage received)
    {
        var receivedJson = System.Text.Encoding.UTF8.GetString(received.Message.Body);
        var msg = CymaticLabs.Unity3D.Amqp.SimpleJSON.JSON.Parse(receivedJson);
    }
}
