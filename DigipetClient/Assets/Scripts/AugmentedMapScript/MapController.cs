using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using CymaticLabs.Unity3D.Amqp;

public class MapController : MonoBehaviour {

    // checker properties for connection and response
    private bool mapAcquiredAndProcessed;

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
    private GameObject petObject;

    List<OtherPlayerData> otherPlayerDataList;

    //private string fileName = "MapData.txt";
    //private string filePath = "";
    private bool firstStart = false;

    // game object for route button
    private GameObject routeObject;

    // Use this for initialization
    void Start () {
        otherPlayerDataList = new List<OtherPlayerData>();

        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        petObject = GameObject.Find("cat_Walk");

        routeObject = GameObject.Find("RouteButton");
        routeObject.GetComponent<Button>().onClick.AddListener(StartRouting);

        this.InitDefaultProperties();
        //StartCoroutine(this.StartGPS());
    }
	
	// Update is called once per frame
	void Update () {

        //Debug.Log(this.responseAcquiredAndProcessed);
        if (mapAcquiredAndProcessed)
        {
            this.UpdateGpsAndSendRequest();
        }

        //UpdatePetPos();

        CheckAndProcessResponse();
        
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
        this.playerName = DataControllerScript.dataController.username; //"User";
        this.latitude = -6.8899f; //-6.893126f; 
        this.longitude = 107.61f; //107.6278f; 
        this.lastLatitude = float.MinValue;
        this.lastLongitude = float.MinValue;
        this.petName = DataControllerScript.dataController.petName; //"Pet";
        this.petPosX = 0f;
        this.petPosY = 0f;

        TextMesh textMesh = petObject.GetComponentInChildren<TextMesh>();
        textMesh.text = DataControllerScript.dataController.petName;

        this.mapAcquiredAndProcessed = true;
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

        Input.location.Start(0.1f, 0.1f);

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

            Debug.Log("publish map");

            string requestJson = this.CreateJsonMessage("map");
            AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);

            this.mapAcquiredAndProcessed = false;
        }    
    }

    void UpdatePetPos()
    {
        this.petPosX = petObject.transform.position.x;
        this.petPosY = petObject.transform.position.z;

        string requestJson = CreateUpdatePetPosJsonMessage("listplayer");
        AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);
    }

    void StartRouting()
    {
        GameObject textObject = GameObject.Find("DestinationField");
        string destination = textObject.GetComponent<InputField>().text;

        string routeRequestJson = this.CreateRouteJsonMessage("route", destination);
        AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, routeRequestJson);
    }

    void CheckAndProcessResponse()
    {
        var msg = AmqpController.amqpControl.msg;
        if (msg != null) // check for msg
        {
            string id = (string)msg["id"];
            if (id == this.uniqueId) // check for guid or unique id
            {
                string responseType = (string)msg["type"];
                if (responseType == "map") // response for create map
                {
                    bool createNewMap = (bool)msg["needCreateMap"];
                    if (createNewMap)
                    {
                        Debug.Log("CREATE MAP");
                        this.DestroyGameObjectByTagName("MapObject");

                        var buildingData = msg["mapData"]["listBuildingData"];
                        this.CreateBuilding(buildingData);
                        var roadData = msg["mapData"]["listRoadData"];
                        this.CreateRoad(roadData);
                    }else
                    {
                        if (firstStart)
                        {
                            Debug.Log("FIRST START CREATE MAP");
                            this.DestroyGameObjectByTagName("MapObject");

                            var buildingData = msg["mapData"]["listBuildingData"];
                            this.CreateBuilding(buildingData);
                            var roadData = msg["mapData"]["listRoadData"];
                            this.CreateRoad(roadData);

                            firstStart = false;
                        }
                    }

                    Vector3 tempCamPos = this.mainCam.transform.position;
                    this.mainCam.transform.position = new Vector3((float)msg["playerPosX"], tempCamPos.y, (float)msg["playerPosY"]);
                    this.mapAcquiredAndProcessed = true;

                    AmqpController.amqpControl.msg = null;
                }

                if (responseType == "route") // response for create route
                {
                    Debug.Log("CREATE ROUTE");
                    this.DestroyGameObjectByTagName("RouteObject");
                    CreateRoute(msg["route"]);

                    AmqpController.amqpControl.msg = null;
                }

                if (responseType == "listPlayer")
                {
                    Debug.Log("listplayer");
                    UpdateOthersPosition(msg["unityPlayerPos"]);

                    AmqpController.amqpControl.msg = null;
                }
            }
        }
    }

    
    /*
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

                        // old json version
                        //var buildingData = msg["mapData"]["buildings"]["features"];
                        //this.CreateBuilding(buildingData);
                        //var roadData = msg["mapData"]["roads"]["features"];
                        //this.CreateRoad(roadData);
                        //var poiData = msg["mapData"]["pois"]["features"];
                        //this.CreatePOI(poiData);

                        // new json version
                        var buildingData = msg["mapData"]["listBuildingData"];
                        this.CreateBuilding(buildingData);
                        var roadData = msg["mapData"]["listRoadData"];
                        this.CreateRoad(roadData);

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

                            // old json version
                            //var buildingData = msg["mapData"]["buildings"]["features"];
                            //this.CreateBuilding(buildingData);
                            //var roadData = msg["mapData"]["roads"]["features"];
                            //this.CreateRoad(roadData);
                            //var poiData = msg["mapData"]["pois"]["features"];
                            //this.CreatePOI(poiData);

                            var buildingData = msg["mapData"]["listBuildingData"];
                            this.CreateBuilding(buildingData);
                            var roadData = msg["mapData"]["listRoadData"];
                            this.CreateRoad(roadData);

                            this.firstStart = false;
                        }
                    }

                    Vector3 tempCamPos = this.mainCam.transform.position;
                    this.mainCam.transform.position = new Vector3((float)msg["playerPosX"], tempCamPos.y, (float)msg["playerPosY"]);
                    this.responseAcquiredAndProcessed = true;
                }
            }

            // create route path
            if ((int)msg["type"] == 3)
            {
                if ((string)msg["id"] == this.uniqueId)
                {
                    Debug.Log("CREATE ROUTE");
                    CreateRoute(msg["route"]);
                }
            }

            // create or update pet for other player in range
            if ((int)msg["type"] == 6)
            {

            }
        }

        //Debug.Log("Request Exchange Message : " + receivedJson);
    }
    */

    void UpdateOthersPosition(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode data)
    {
        for (int i=0; i<data.Count; i++)
        {
            string otherUsername = (string)data[i]["playerName"];
            float otherPosX = (float)data[i]["posX"];
            float otherPosY = (float)data[i]["posY"];
            string otherPetName = (string)data[i]["petName"];
            float otherPetPosX = (float)data[i]["petPosX"];
            float otherPetPosY = (float)data[i]["petPosY"];

            OtherPlayerData other = otherPlayerDataList.Find(x => x.playerName == otherUsername);
            if (other == null)
            {
                OtherPlayerData newOtherPlayer = new OtherPlayerData();
                newOtherPlayer.playerName = otherUsername;
                newOtherPlayer.posX = otherPosX;
                newOtherPlayer.posY = otherPosY;
                newOtherPlayer.petName = otherPetName;
                newOtherPlayer.petPosX = otherPetPosX;
                newOtherPlayer.petPosY = otherPetPosY;

                otherPlayerDataList.Add(newOtherPlayer);

                // belum ada nama pet
                GameObject NewPetObject = Instantiate(Resources.Load("PetPrefab")) as GameObject;
                NewPetObject.name = otherPetName;
                NewPetObject.transform.position = new Vector3(otherPetPosX, 0.0f, otherPetPosY);

                GameObject otherPlayerNameObject = new GameObject();
                otherPlayerNameObject.name = otherUsername;
                otherPlayerNameObject.transform.position = new Vector3(otherPosX, 0.0f, otherPosY);
                var meshText = otherPlayerNameObject.AddComponent<TextMesh>() as TextMesh;
                meshText.text = otherUsername;
            }
            else
            {
                other.petPosX = otherPetPosX;
                other.petPosY = otherPetPosY;
                GameObject curPetObject = GameObject.Find(otherPetName);
                //curPetObject.GetComponent<PetController2>().enabled = false;

                Vector3 newDir = Vector3.RotateTowards(curPetObject.transform.forward, new Vector3(otherPetPosX, 0.0f, otherPetPosY), Time.deltaTime * 0.1f, 0.0f);
                curPetObject.transform.rotation = Quaternion.LookRotation(newDir);

                curPetObject.transform.position = Vector3.MoveTowards(curPetObject.transform.position, new Vector3(otherPetPosX, 0.0f, otherPetPosY), Time.deltaTime * 0.1f);
            }
        }

    }

    void CreateRoute(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode routeData)
    {
        for (int i=0; i<routeData.Count-1; i++)
        {
            float startLat = (float)routeData[i]["latitude"];
            float startLon = (float)routeData[i]["longitude"];
            Vector3 start = new Vector3(startLat, 0.0f, startLon);

            float endLat = (float)routeData[i+1]["latitude"];
            float endLon = (float)routeData[i+1]["longitude"];
            Vector3 end = new Vector3(endLat, 0.0f, endLon);

            CreateRoadWaterMesh(start, end, 2.0f, "RouteObject", Color.green, "route");
        }
    }

    // create building's mesh and name based on data from response
    void CreateBuilding(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode buildingData)
    {
        // old create building version
        //List<Vector3> point = new List<Vector3>();
        //List<Vector2> point2d = new List<Vector2>();
        //for (int i=0; i<buildingData.Count; i++)
        //{
        //    var tempData = buildingData[i];
        //    if ((string)tempData["geometry"]["type"] == "Polygon")
        //    {
        //        for (int j=0; j<tempData["geometry"]["coordinates"][0].Count - 1; j++)
        //        {
        //            var coordinate = tempData["geometry"]["coordinates"][0][j];
        //            point.Add(new Vector3((float)coordinate[1], 0.0f, (float)coordinate[0]));
        //            point2d.Add(new Vector2((float)coordinate[0], (float)coordinate[1]));
        //        }

        //        this.CreatePolygon(point2d.ToArray(), point.ToArray(), Color.green, "MapObject", "building");

        //        point.Clear();
        //        point2d.Clear();
        //    }
        //    if ((string)tempData["geometry"]["type"] == "Point")
        //    {
        //        string buildingNames = (string)tempData["properties"]["name"];
        //        var coordinate = tempData["geometry"]["coordinates"];
        //        this.ShowName(new Vector3((float)coordinate[1], 0.0f, (float)coordinate[0]), new Vector3(), buildingNames, "MapObject", "buildingName", Color.black);
        //    }
        //}

        // new create building version
        List<Vector3> point = new List<Vector3>();
        List<Vector2> point2d = new List<Vector2>();
        for (int i=0; i<buildingData.Count; i++)
        {
            var tempData = buildingData[i];
            if (tempData["listCoordinate"].Count == 1)
            {
                string buildingName = (string)tempData["buildingName"];
                var coordinate = tempData["listCoordinate"][0];
                this.ShowName(new Vector3((float)coordinate["latitude"], 1.75f, (float)coordinate["longitude"]), new Vector3(), buildingName, "MapObject", "buildingName", Color.red);
            }
            else
            {
                for (int j = 0; j < tempData["listCoordinate"].Count-1; j++)
                {
                    var coordinate = tempData["listCoordinate"][j];
                    float latitude = (float)coordinate["latitude"];
                    float longitude = (float)coordinate["longitude"];
                    point.Add(new Vector3(latitude, 0.0f, longitude));
                    point2d.Add(new Vector2(latitude, longitude));
                }

                this.CreatePolygon(point2d.ToArray(), point.ToArray(), Color.green, "MapObject", "building");

                point.Clear();
                point2d.Clear();
            }
        }
    }

    // create road's mesh and name based on response 
    void CreateRoad(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode roadData)
    {
        // old create road version
        //List<Vector3[]> point = new List<Vector3[]>();
        //List<string> names = new List<string>();
        //for (int i=0; i<roadData.Count; i++)
        //{
        //    var tempData = roadData[i];
        //    if ((string)tempData["geometry"]["type"] == "LineString")
        //    {
        //        Vector3[] perPoint = new Vector3[tempData["geometry"]["coordinates"].Count];
        //        for (int j=0; j< tempData["geometry"]["coordinates"].Count; j++)
        //        {
        //            var coordinate = tempData["geometry"]["coordinates"][j];
        //            perPoint[j] = new Vector3((float)coordinate[1], 0.1f, (float)coordinate[0]);
        //        }

        //        point.Add(perPoint);
        //        names.Add((string)tempData["properties"]["name"]);
        //    }
        //}

        //for (int k = 0; k < point.Count; k++)
        //{
        //    string roadName = names[k];
        //    Vector3[] tempVector = point[k];
        //    for (int l = 0; l < tempVector.Length - 1; l++)
        //    {
        //        CreateRoadWaterMesh(tempVector[l], tempVector[l + 1], 2.0f, "MapObject", Color.red, "road");
        //        this.ShowName(tempVector[l], tempVector[l + 1], roadName, "MapObject", "roadName", Color.black);
        //    }
        //}

        // new create road version
        List<Vector3[]> point = new List<Vector3[]>();
        List<string> names = new List<string>();
        for (int i=0; i<roadData.Count; i++)
        {
            var tempData = roadData[i];
            Vector3[] perPoint = new Vector3[tempData["listCoordinate"].Count];
            for (int j=0; j<tempData["listCoordinate"].Count; j++)
            {
                var coordinate = tempData["listCoordinate"][j];
                float latitude = (float)coordinate["latitude"];
                float longitude = (float)coordinate["longitude"];
                perPoint[j] = new Vector3(latitude, 0.1f, longitude);
            }

            point.Add(perPoint);
            names.Add((string)tempData["roadName"]);
        }

        for (int k = 0; k < point.Count; k++)
        {
            string roadName = names[k];
            Vector3[] tempVector = point[k];
            for (int l = 0; l < tempVector.Length - 1; l++)
            {
                CreateRoadWaterMesh(tempVector[l], tempVector[l + 1], 2.0f, "MapObject", Color.red, "road");
                this.ShowName(tempVector[l], tempVector[l + 1], roadName, "MapObject", "roadName", Color.blue);
            }
        }
    }

    /*
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
    */

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

        mf.mesh = m;
        //mf.mesh = RevertNormals(m);
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

        var text = go.AddComponent<TextMesh>();
        text.text = objectName;
        text.color = color;
        text.characterSize = 0.5f;
        text.anchor = TextAnchor.MiddleCenter;

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
            text.characterSize = 1.0f;
        }
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
    string CreateJsonMessage(string type)
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

    string CreateRouteJsonMessage(string type, string destination)
    {
        RouteRequestJson routeRequestJson = new RouteRequestJson();
        routeRequestJson.id = this.uniqueId;
        routeRequestJson.username = this.playerName;
        routeRequestJson.type = type;
        routeRequestJson.latitude = this.latitude;
        routeRequestJson.longitude = this.longitude;
        routeRequestJson.destination = destination;

        return JsonUtility.ToJson(routeRequestJson);
    }

    string CreateUpdatePetPosJsonMessage(string type)
    {
        UpdatePetPosJson updatePetRequest = new UpdatePetPosJson();
        updatePetRequest.id = this.uniqueId;
        updatePetRequest.type = type;
        updatePetRequest.username = this.playerName;
        updatePetRequest.petPosX = this.petPosX;
        updatePetRequest.petPosY = this.petPosY;

        return JsonUtility.ToJson(updatePetRequest);
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
        public string id; 
        public string type;
        public string playerName;
        public float latitude;
        public float longitude;  
        public string petName;
        public float petPosX;    
        public float petPosY;       
    }

    [Serializable]
    public class RouteRequestJson
    {
        public string id;
        public string username;
        public string type;
        public float latitude;
        public float longitude;
        public string destination;
    }

    [Serializable]
    public class UpdatePetPosJson
    {
        public string id;
        public string type;
        public string username;
        public float petPosX;
        public float petPosY;
    }

    public class OtherPlayerData
    {
        public string playerName;
        public float posX;
        public float posY;
        public string petName;
        public float petPosX;
        public float petPosY;
    }
}
