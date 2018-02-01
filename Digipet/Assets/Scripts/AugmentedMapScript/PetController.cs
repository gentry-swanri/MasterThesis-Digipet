using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using CymaticLabs.Unity3D.Amqp;
using System;

public class PetController : MonoBehaviour {

    //private int timeBeforeRotate;
    private float speed;
    private bool foodEnabled;

    private Animator anim;
    private bool isEating;
    private int eatTime;

    private Vector3 targetMove;
    private string timeStartMove;

    private GameObject mapController;

    // Use this for initialization
    void Start()
    {
        //timeBeforeRotate = Random.Range(500, 1000);
        mapController = GameObject.Find("Map");
        timeStartMove = "";

        targetMove = transform.position;
        //RandomTargetPos();
        //Debug.Log(targetMove);
        speed = 1.0f;
        foodEnabled = false;
        isEating = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Pet COntroller");
        MoveBehaviour();
        GiveFood();
        MoveToEat();
        Eating();
    }

    void MoveBehaviour()
    {
        if (!foodEnabled)
        {
            /*
            //Debug.Log("masuk move");
            //Debug.Log(timeBeforeRotate);
            if (timeBeforeRotate < 0)
            {
                int degreeRot = Random.Range(0, 360);
                //Debug.Log(degreeRot);
                this.transform.Rotate(0, degreeRot, 0);

                timeBeforeRotate = Random.Range(500, 1000);
            }
            else
            {
                this.transform.position += this.transform.forward * Time.deltaTime * speed;
                timeBeforeRotate--;
            }
            */
            
            
            var mapScript = mapController.GetComponent<MapController>();
            int tileX = mapScript.tileX;
            int tileY = mapScript.tileY;
            bool readyToSend = mapScript.okToSentPetPos;

            if (Vector3.Distance(targetMove, transform.position) < 0.1f && readyToSend)
            {
                Debug.Log("Ready send = "+readyToSend);
                Debug.Log("Tile X = "+tileX);
                Debug.Log("Tile Y = "+tileY);

                RandomTargetPos();
                timeStartMove = DateTime.Now.Ticks.ToString();
                
                UpdatePetPos petPos = new UpdatePetPos();
                petPos.type = "listplayer";
                petPos.username = PlayerPrefs.GetString("username");
                petPos.timeStartMove = timeStartMove;
                petPos.petPosX = targetMove.x;
                petPos.petPosY = targetMove.z;
                petPos.tileX = tileX;
                petPos.tileY = tileY;
                petPos.petState = "walk";

                string requestJson = JsonUtility.ToJson(petPos);
                AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);
                
            }

            

            Vector3 lookpos = targetMove - transform.position;
            lookpos.y = 0;
            if (lookpos != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(lookpos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * speed);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetMove, Time.deltaTime * speed);
        }
    }

    void GiveFood()
    {
        if (!foodEnabled)
        {
            /*if(Input.GetMouseButtonDown(0))*/if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                /*if (!EventSystem.current.IsPointerOverGameObject())*/  if(!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    GameObject mainCam = GameObject.FindGameObjectWithTag("MainCamera");

                    GameObject food = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    food.tag = "Food";
                    food.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    food.transform.position = new Vector3(mainCam.transform.parent.position.x, 0, mainCam.transform.parent.position.z);
                    //food.transform.position = new Vector3(Random.Range(-10, 10), 0.0f, Random.Range(-10, 10));
                    Rigidbody foodRigibody = food.AddComponent<Rigidbody>();
                    foodRigibody.mass = 1;
                    foodRigibody.isKinematic = true;

                    foodEnabled = true;

                    // update position in server
                    var mapScript = mapController.GetComponent<MapController>();
                    int tileX = mapScript.tileX;
                    int tileY = mapScript.tileY;
                    bool readyToSend = mapScript.okToSentPetPos;

                    if (readyToSend)
                    {

                        timeStartMove = DateTime.Now.Ticks.ToString();

                        UpdatePetPos petPos = new UpdatePetPos();
                        petPos.type = "listplayer";
                        petPos.username = PlayerPrefs.GetString("username");
                        petPos.timeStartMove = timeStartMove;
                        petPos.petPosX = food.transform.position.x;
                        petPos.petPosY = food.transform.position.z;
                        petPos.tileX = tileX;
                        petPos.tileY = tileY;
                        petPos.petState = "walkFood";

                        string requestJson = JsonUtility.ToJson(petPos);
                        AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);
                    }
                }
                    
            }
        }
    }

    void MoveToEat()
    {
        if (foodEnabled)
        {
            GameObject food = GameObject.FindGameObjectWithTag("Food");

            Vector3 lookpos = food.transform.position - this.transform.position;
            lookpos.y = 0;
            if (lookpos != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(lookpos);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime * speed);
            }  

            this.transform.position = Vector3.MoveTowards(this.transform.position, food.transform.position, Time.deltaTime * speed);
        }
    }

    void Eating()
    {
        if (isEating)
        {
            if (eatTime > 0)
            {
                eatTime--;
            }else
            {
                int curEnergy = PlayerPrefs.GetInt("Food");
                int curAgility = PlayerPrefs.GetInt("Walk");
                PlayerPrefs.SetInt("Food", curEnergy+2);
                PlayerPrefs.SetInt("Walk", curAgility+2);

                anim.runtimeAnimatorController = Resources.Load("AnimationController/PetWalkController") as RuntimeAnimatorController;
                eatTime = 160;
                isEating = false;

                foodEnabled = false;
                //timeBeforeRotate = -1;
                targetMove = transform.position;
                Destroy(GameObject.FindGameObjectWithTag("Food"));
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Masuk Collision");
        if (col.gameObject.tag == "Food")
        {
            //Debug.Log("eat food");
            anim = GameObject.Find("cu_puppy_shiba_a").GetComponent<Animator>();
            anim.runtimeAnimatorController = Resources.Load("AnimationController/PetEatController") as RuntimeAnimatorController;
            isEating = true;
            eatTime = 160;

            //if (DataController.HUNGER < 100)
            //{
            //    DataController.HUNGER++;
            //}

            var mapScript = mapController.GetComponent<MapController>();
            int tileX = mapScript.tileX;
            int tileY = mapScript.tileY;
            bool readyToSend = mapScript.okToSentPetPos;

            if (readyToSend)
            {

                timeStartMove = DateTime.Now.Ticks.ToString();

                UpdatePetPos petPos = new UpdatePetPos();
                petPos.type = "listplayer";
                petPos.username = PlayerPrefs.GetString("username");
                petPos.timeStartMove = timeStartMove;
                petPos.petPosX = this.transform.position.x;
                petPos.petPosY = this.transform.position.z;
                petPos.tileX = tileX;
                petPos.tileY = tileY;
                petPos.petState = "eatFood";

                string requestJson = JsonUtility.ToJson(petPos);
                AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);
            }
        }
    }

    void RandomTargetPos()
    {
        float x = 0.0f;
        float z = 0.0f;

        int sign = UnityEngine.Random.Range(0, 2);
        if (sign == 0)
        {
            x = UnityEngine.Random.Range(-100, -50);
        }else
        {
            x = UnityEngine.Random.Range(50, 100);
        }

        sign = UnityEngine.Random.Range(0, 2);
        if (sign == 0)
        {
            z = UnityEngine.Random.Range(-100, -50);
        }else
        {
            z = UnityEngine.Random.Range(50, 100);
        }

        targetMove.Set(x, 0.0f, z);

        //Debug.Log("X = "+x);
        //Debug.Log("Z = "+z);
    }

    [Serializable]
    class UpdatePetPos
    {
        public string type;
        public string username;
        public string timeStartMove;
        public float petPosX;
        public float petPosY;
        public int tileX;
        public int tileY;
        public string petState;
    }
}
