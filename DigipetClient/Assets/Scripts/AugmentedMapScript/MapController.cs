using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using CymaticLabs.Unity3D.Amqp;

public class MapController : MonoBehaviour {

    // request properties section
    private string requestExchangeName;
    private string requestRoutingKey;

    private string responseExchangeName;
    private string responseRoutingKey;
    private AmqpExchangeTypes responseExchangeType;

    // checker properties for connection and response
    private bool serverConnected;
    private bool responseAcquiredAndProcessed;

    // class properties
    private string uniqueId;
    private string playerName;
    private float latitude;
    private float longitude;
    private float lastLatitude;
    private float lastLongitude;
    private string petName;
    private float petPosX;
    private float petPosY;
    private GameObject mainCam;

    private string fileName = "MapData.txt";
    private string filePath = "";
    private bool firstStart = false;

    // game object for route button
    private GameObject routeObject;

    // Use this for initialization
    void Start () {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");

        routeObject = GameObject.Find("RouteButton");
        routeObject.GetComponent<Button>().onClick.AddListener(StartRouting);

        this.InitDefaultProperties();
        //StartCoroutine(this.StartGPS());
        this.StartConnection();
    }
	
	// Update is called once per frame
	void Update () {
        
        if (serverConnected)
        {
            //Debug.Log(this.responseAcquiredAndProcessed);
            if (responseAcquiredAndProcessed)
            {
                this.UpdateGpsAndSendRequest();
            }           

            //serverConnected = false;
        }
        
	}

    void OnApplicationPause()
    {
        //string requestJson = this.CreateJsonMessage(3);
        //AmqpClient.Publish(this.requestExchangeName, this.requestRoutingKey, requestJson);
    }

    // initialize all properties with default value
    void InitDefaultProperties()
    {
        this.uniqueId = Guid.NewGuid().ToString();
        this.playerName = "User";
        this.latitude = -6.8899f;
        this.longitude = 107.61f;
        this.lastLatitude = float.MinValue;
        this.lastLongitude = float.MinValue;
        this.petName = "Pet";
        this.petPosX = 0f;
        this.petPosY = 0f;

        this.serverConnected = false;
        this.responseAcquiredAndProcessed = true;
        this.firstStart = true;
    }

    // start gps and get both latitude and longitude value
    IEnumerator StartGPS()
    {
        if (!Input.location.isEnabledByUser)
        {
            print("GPS not enabled");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            print("Timed Out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine location");
            yield break;
        }
        else
        {
            this.latitude = Input.location.lastData.latitude;
            this.longitude = Input.location.lastData.longitude;
        }
    }

    // start connection to rabbitmq server
    void StartConnection()
    {
        // initialize request properties
        this.requestExchangeName = "DigipetRequestExchange";
        this.requestRoutingKey = "DigipetRequestRoutingKey";

        this.responseExchangeName = "DigipetResponseExchange";
        this.responseRoutingKey = "DigipetResponseRoutingKey";
        this.responseExchangeType = AmqpExchangeTypes.Direct;

        // connect to rabbitmq server
        AmqpClient.Instance.ConnectOnStart = false;
        AmqpClient.Connect();
        
        // handle event after connected to rabbitmq server
        AmqpClient.Instance.OnConnected.AddListener(HandleConnected);

        //Debug.Log("server has connected");
    }

    // update latitude and longitude value and send the request to server through rabbitmq
    void UpdateGpsAndSendRequest()
    {
        //this.latitude = Input.location.lastData.latitude;
        //this.longitude = Input.location.lastData.longitude;

        //this.latitude -= 0.00001f;
        if (this.lastLatitude != this.latitude || this.lastLongitude != this.longitude)
        {
            //Debug.Log(this.lastLatitude + " ------- " + this.latitude);
            //Debug.Log(this.lastLongitude + " ------- " + this.longitude);

            this.lastLatitude = this.latitude;
            this.lastLongitude = this.longitude;

            string requestJson = this.CreateJsonMessage(1);
            AmqpClient.Publish(this.requestExchangeName, this.requestRoutingKey, requestJson);

            this.responseAcquiredAndProcessed = false;
        }
            
    }

    void StartRouting()
    {
        GameObject textObject = GameObject.Find("DestinationField");
        string destination = textObject.GetComponent<InputField>().text;

        string routeRequestJson = this.CreateRouteJsonMessage(5, destination);
        AmqpClient.Publish(this.requestExchangeName, this.requestRoutingKey, routeRequestJson);
    }

    // subscribe to rabbitmq exchange if connection is successful
    void HandleConnected(AmqpClient clientParam)
    {
        // subscribe to exchange for request purpose
        var exchangeSubscription = new AmqpExchangeSubscription(responseExchangeName, responseExchangeType, responseRoutingKey, HandleExchangeMessageReceived);       
        AmqpClient.Subscribe(exchangeSubscription);

        // testing publish message to server
        // AmqpClient.Publish(this.requestExchangeName, this.requestRoutingKey, "test");

        // set connect status to true
        this.serverConnected = true;
    }

    // received response from server and process it
    void HandleExchangeMessageReceived(AmqpExchangeReceivedMessage received)
    {
        var receivedJson = System.Text.Encoding.UTF8.GetString(received.Message.Body);
        var msg = CymaticLabs.Unity3D.Amqp.SimpleJSON.JSON.Parse(receivedJson);

        if (msg != null)
        {
            // create map
            if ((int)msg["type"] == 1)
            {
                if ((string)msg["id"] == this.uniqueId)
                {
                    if ((bool)msg["needCreateMap"])
                    {
                        Debug.Log("CREATE MAP");
                        this.DestroyGameObjectByTagName("MapObject");

                        var buildingData = msg["mapData"]["buildings"]["features"];
                        this.CreateBuilding(buildingData);
                        var roadData = msg["mapData"]["roads"]["features"];
                        this.CreateRoad(roadData);
                        var poiData = msg["mapData"]["pois"]["features"];
                        this.CreatePOI(poiData);

                        //this.WriteFile(receivedJson);
                    }
                    else
                    {
                        //if (firstStart)
                        //{
                        //    string json = this.ReadFile();
                        //    var msgLoaded = CymaticLabs.Unity3D.Amqp.SimpleJSON.JSON.Parse(json);

                        //    this.DestroyGameObjectByTagName("MapObject");

                        //    var buildingData = msgLoaded["mapData"]["buildings"]["features"];
                        //    this.CreateBuilding(buildingData);
                        //    var roadData = msgLoaded["mapData"]["roads"]["features"];
                        //    this.CreateRoad(roadData);
                        //    var poiData = msgLoaded["mapData"]["pois"]["features"];
                        //    this.CreatePOI(poiData);

                        //    this.firstStart = false;
                        //}

                        if (firstStart)
                        {
                            Debug.Log("CREATE MAP FIRST START");
                            this.DestroyGameObjectByTagName("MapObject");

                            var buildingData = msg["mapData"]["buildings"]["features"];
                            this.CreateBuilding(buildingData);
                            var roadData = msg["mapData"]["roads"]["features"];
                            this.CreateRoad(roadData);
                            var poiData = msg["mapData"]["pois"]["features"];
                            this.CreatePOI(poiData);

                            this.firstStart = false;
                        }
                    }

                    Vector3 tempCamPos = this.mainCam.transform.position;
                    this.mainCam.transform.position = new Vector3((float)msg["playerPosX"], tempCamPos.y, (float)msg["playerPosY"]);
                    this.responseAcquiredAndProcessed = true;
                }
            }

            // create route path
            if ((int)msg["type"] == 5)
            {
                if ((string)msg["id"] == this.uniqueId)
                {
                    
                }
            }

            // create or update pet for other player in range
            if ((int)msg["type"] == 6)
            {

            }
        }

        //Debug.Log("Request Exchange Message : " + receivedJson);
    }

    // create building's mesh and name based on data from response
    void CreateBuilding(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode buildingData)
    {
        List<Vector3> point = new List<Vector3>();
        List<Vector2> point2d = new List<Vector2>();
        for (int i=0; i<buildingData.Count; i++)
        {
            var tempData = buildingData[i];
            if ((string)tempData["geometry"]["type"] == "Polygon")
            {
                for (int j=0; j<tempData["geometry"]["coordinates"][0].Count - 1; j++)
                {
                    var coordinate = tempData["geometry"]["coordinates"][0][j];
                    point.Add(new Vector3((float)coordinate[1], 0.0f, (float)coordinate[0]));
                    point2d.Add(new Vector2((float)coordinate[0], (float)coordinate[1]));
                }

                this.CreatePolygon(point2d.ToArray(), point.ToArray(), Color.green, "MapObject", "building");

                point.Clear();
                point2d.Clear();
            }
            if ((string)tempData["geometry"]["type"] == "Point")
            {
                string buildingNames = (string)tempData["properties"]["name"];
                var coordinate = tempData["geometry"]["coordinates"];
                this.ShowName(new Vector3((float)coordinate[1], 0.0f, (float)coordinate[0]), new Vector3(), buildingNames, "MapObject", "buildingName", Color.black);
            }
        }
    }

    // create road's mesh and name based on response 
    void CreateRoad(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode roadData)
    {
        List<Vector3[]> point = new List<Vector3[]>();
        List<string> names = new List<string>();
        for (int i=0; i<roadData.Count; i++)
        {
            var tempData = roadData[i];
            if ((string)tempData["geometry"]["type"] == "LineString")
            {
                Vector3[] perPoint = new Vector3[tempData["geometry"]["coordinates"].Count];
                for (int j=0; j< tempData["geometry"]["coordinates"].Count; j++)
                {
                    var coordinate = tempData["geometry"]["coordinates"][j];
                    perPoint[j] = new Vector3((float)coordinate[1], 0.1f, (float)coordinate[0]);
                }

                point.Add(perPoint);
                names.Add((string)tempData["properties"]["name"]);
            }
        }

        for (int k = 0; k < point.Count; k++)
        {
            string roadName = names[k];
            Vector3[] tempVector = point[k];
            for (int l = 0; l < tempVector.Length - 1; l++)
            {
                CreateRoadWaterMesh(tempVector[l], tempVector[l + 1], 2.0f, "MapObject", Color.red, "road");
                this.ShowName(tempVector[l], tempVector[l + 1], roadName, "MapObject", "roadName", Color.black);
            }
        }
    }

    // create poi's name based on response
    void CreatePOI(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode poiData)
    {
        for (int i=0; i<poiData.Count; i++)
        {
            var tempData = poiData[i];
            var coordinate = tempData["geometry"]["coordinates"];
            var poiName = (string)tempData["properties"]["name"];
            if (poiName != null)
            {
                this.ShowName(new Vector3((float)coordinate[1], 0.0f, (float)coordinate[0]), new Vector3(), poiName, "MapObject", "poiName", Color.black);
            }
        }
    }

    // create building mesh
    void CreatePolygon(Vector2[] points2D, Vector3[] points, Color color, string tagName, string typeName)
    {
        Triangulator tr = new Triangulator(points2D);
        int[] indices = tr.Triangulate();

        GameObject go = new GameObject(typeName + "_mesh");
        go.tag = tagName;
        var mf = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
        var mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        mr.material.color = color;

        Mesh m = new Mesh();
        m.Clear();

        m.vertices = points;

        m.triangles = indices;

        //mf.mesh = m;
        mf.mesh = RevertNormals(m);
        m.RecalculateBounds();
        m.RecalculateNormals();
        //MeshUtility.Optimize(m);
    }

    // create road mesh
    void CreateRoadWaterMesh(Vector3 start, Vector3 end, float lineWidth, string tagName, Color color, string typeName)
    {
        // create main road mesh
        Vector3 normal = Vector3.Cross(start, end);
        Vector3 side = Vector3.Cross(normal, end - start);
        side.Normalize();
        Vector3 a = start + side * (lineWidth / 2);
        Vector3 b = start + side * (lineWidth / -2);
        Vector3 c = end + side * (lineWidth / 2);
        Vector3 d = end + side * (lineWidth / -2);

        Vector3[] points = new Vector3[] { a, b, c, d };

        GameObject go = new GameObject(typeName + "_mesh");
        go.tag = tagName;
        var mf = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
        var mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        mr.material.color = color;

        Mesh m = new Mesh();
        m.Clear();

        m.vertices = points;

        m.triangles = new int[] { 0, 2, 1, 2, 3, 1 };

        if (normal.y > 0)
        {
            mf.mesh = RevertNormals(m);
        }
        else
        {
            mf.mesh = m;
        }

        //mf.mesh = m;
        //mf.mesh = RevertNormals(m);
        m.RecalculateBounds();
        m.RecalculateNormals();
        //MeshUtility.Optimize(m);
    }

    // show the name of building, road, and poi
    void ShowName(Vector3 textPosStart, Vector3 textPosEnd, string objectName, string tagName, string typeName, Color color)
    {
        GameObject go = new GameObject(typeName + "_mesh");
        go.tag = tagName;
        if (typeName == "roadName")
        {
            go.transform.position = Vector3.Lerp(textPosStart, textPosEnd, 0.5f);
            Vector3 direction = (textPosEnd - textPosStart).normalized;
            go.transform.rotation = Quaternion.LookRotation(direction);
            go.transform.Rotate(new Vector3(90, textPosEnd.z < textPosStart.z ? -90 : 90, 0));
        }else if (typeName == "buildingName" || typeName == "poiName")
        {
            go.transform.position = textPosStart;
            go.AddComponent<NameController>();
        }

        var text = go.AddComponent<TextMesh>();
        text.text = objectName;
        text.color = color;
        text.characterSize = 2.5f;
    }

    // revert the normals of mesh
    Mesh RevertNormals(Mesh mesh)
    {
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] = -normals[i];
        mesh.normals = normals;

        for (int m = 0; m < mesh.subMeshCount; m++)
        {
            int[] triangles = mesh.GetTriangles(m);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int temp = triangles[i + 0];
                triangles[i + 0] = triangles[i + 1];
                triangles[i + 1] = temp;
            }
            mesh.SetTriangles(triangles, m);
        }

        return mesh;
    }

    // destroy all game object based on specific tag name
    void DestroyGameObjectByTagName(string tagName)
    {
        GameObject[] arrGO = GameObject.FindGameObjectsWithTag(tagName);
        for (int i = 0; i < arrGO.Length; i++)
        {
            Destroy(arrGO[i]);
        }
    }

    // create json string to be used as request message
    string CreateJsonMessage(int type)
    {
        RequestJSON requestJson = new RequestJSON();
        requestJson.id = this.uniqueId;
        requestJson.type = type;
        requestJson.playerName = this.playerName;
        requestJson.latitude = this.latitude;
        requestJson.longitude = this.longitude;
        requestJson.petName = this.petName;
        requestJson.petPosX = this.petPosX;
        requestJson.petPosY = this.petPosY;

        return JsonUtility.ToJson(requestJson);
    }

    string CreateRouteJsonMessage(int type, string destination)
    {
        RouteRequestJson routeRequestJson = new RouteRequestJson();
        routeRequestJson.id = this.uniqueId;
        routeRequestJson.type = type;
        routeRequestJson.latitude = this.latitude;
        routeRequestJson.longitude = this.longitude;
        routeRequestJson.destination = destination;

        return JsonUtility.ToJson(routeRequestJson);
    }

    //void WriteFile(string msg)
    //{
    //    if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
    //    {
    //        filePath = Application.persistentDataPath;
    //    }
    //    else
    //    {
    //        filePath = Application.dataPath;
    //    }

    //    bool exist = File.Exists(filePath + "/" + fileName);
    //    if (exist)
    //    {
    //        File.Delete(filePath + "/" + fileName);
    //    }

    //    var sr = File.CreateText(filePath + "/" + fileName);
    //    sr.WriteLine(msg);
    //    sr.Close();
    //}

    //string ReadFile()
    //{
    //    string json = "";

    //    if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
    //    {
    //        filePath = Application.persistentDataPath;
    //    }
    //    else
    //    {
    //        filePath = Application.dataPath;
    //    }

    //    bool exist = File.Exists(filePath + "/" + fileName);
    //    if (exist)
    //    {
    //        var sr = File.OpenText(filePath + "/" + fileName);
    //        json = sr.ReadLine();
    //    }

    //    return json;
    //}

    // --------------------------------- Inner class for JSON serialization as rabbitmq message ----------------------------------------------- //

    [Serializable]
    public class RequestJSON
    {
        // unique id as identifier
        public string id;

        // type of json : 1 = request, 2 = response           
        public int type;

        // player's name or username          
        public string playerName;

        // latitude based on gps   
        public float latitude;

        // longitude based on gps      
        public float longitude;

        // pet's name     
        public string petName;

        // position of pet in unity x coordinate      
        public float petPosX;

        // position of pet in unity y coordinate       
        public float petPosY;       
    }

    [Serializable]
    public class RouteRequestJson
    {
        public string id;
        public int type;
        public float latitude;
        public float longitude;
        public string destination;
    }
}
