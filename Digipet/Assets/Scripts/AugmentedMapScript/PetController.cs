using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using CymaticLabs.Unity3D.Amqp;
using System;
using UnityEngine.UI;

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

    // pet action section
    private GameObject ball;
    private bool ballPlay;
    private bool ballThrown;
    private bool ballInGround;
    private bool ballDeliver;

    private bool petCalled;

    private GameObject actionPanel;
    private GameObject actionBut;
    private GameObject foodBut;
    private GameObject callBut;
    private GameObject playBut;
    private bool actionActive;

    // Use this for initialization
    void Start()
    {
        mapController = GameObject.Find("Map");
        timeStartMove = "";

        targetMove = transform.position;
        speed = (float)(PlayerPrefs.GetInt("Walk") / 100.0f) + 1;
        foodEnabled = false;
        isEating = false;

        // action section
        actionBut = GameObject.Find("ActionButton");
        actionBut.GetComponent<Button>().onClick.AddListener(ShowAction);

        foodBut = GameObject.Find("FoodButton");
        foodBut.GetComponent<Button>().onClick.AddListener(GiveFood);

        callBut = GameObject.Find("CallButton");
        callBut.GetComponent<Button>().onClick.AddListener(CallPet);

        playBut = GameObject.Find("PlayButton");
        playBut.GetComponent<Button>().onClick.AddListener(PlayBall);

        actionActive = false;

        actionPanel = GameObject.Find("ActionPanel");
        actionPanel.gameObject.SetActive(actionActive);

        ballPlay = false;
        ballThrown = false;
        ballInGround = false;
        ballDeliver = false;

        petCalled = false;
    }

    // Update is called once per frame
    void Update()
    {
        MoveBehaviour();

        MoveToEat();
        Eating();

        if (ballPlay)
        {
            MoveBallMatchCameraRotation();
            ThrowBall();
            SimulateProjectile();
        }

        MovePlay();
        MoveDeliver();

        if (petCalled)
        {
            CallPetMove();
        }

        speed = (float)(PlayerPrefs.GetInt("Walk") / 100.0f) + 1;
    }

    void MoveBehaviour()
    {
        if (!foodEnabled && !ballInGround && !petCalled)
        {
            var mapScript = mapController.GetComponent<MapController>();
            int tileX = mapScript.tileX;
            int tileY = mapScript.tileY;
            bool readyToSend = mapScript.okToSentPetPos;

            if (Vector3.Distance(targetMove, transform.position) < 0.1f && readyToSend)
            {
                RandomTargetPos();
                timeStartMove = DateTime.Now.Ticks.ToString();
                
                UpdatePetPos petPos = new UpdatePetPos();
                petPos.type = "listplayer";
                petPos.username = PlayerPrefs.GetString("username");
                petPos.timeStartMove = timeStartMove;
                petPos.petLastPosX = 0f;
                petPos.petLastPosY = 0f;
                petPos.petPosX = targetMove.x;
                petPos.petPosY = targetMove.z;
                petPos.tileX = tileX;
                petPos.tileY = tileY;
                petPos.petState = "walk";
                petPos.speed = speed;

                string requestJson = JsonUtility.ToJson(petPos);
                AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);
                
            }

            

            Vector3 lookpos = targetMove - transform.position;
            lookpos.y = 0;
            if (lookpos != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(lookpos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.0f);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetMove, Time.deltaTime * speed);
        }
    }

    void GiveFood()
    {
        if (!foodEnabled && !ballInGround)
        {
            GameObject mainCam = GameObject.FindGameObjectWithTag("MainCamera");

            GameObject food = GameObject.CreatePrimitive(PrimitiveType.Cube);
            food.tag = "Food";
            food.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            food.transform.position = new Vector3(mainCam.transform.parent.position.x, 0, mainCam.transform.parent.position.z);
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
                petPos.petLastPosX = transform.position.x;
                petPos.petLastPosY = transform.position.z;
                petPos.petPosX = food.transform.position.x;
                petPos.petPosY = food.transform.position.z;
                petPos.tileX = tileX;
                petPos.tileY = tileY;
                petPos.petState = "walkFood";
                petPos.speed = speed;

                string requestJson = JsonUtility.ToJson(petPos);
                AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);
            }

            petCalled = false;
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
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime * 2.0f);
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
                PlayerPrefs.SetInt("Food", curEnergy+7);
                PlayerPrefs.SetInt("Walk", curAgility+2);

                anim.runtimeAnimatorController = Resources.Load("AnimationController/PetWalkController") as RuntimeAnimatorController;
                eatTime = 160;
                isEating = false;

                foodEnabled = false;
                targetMove = transform.position;
                Destroy(GameObject.FindGameObjectWithTag("Food"));
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Food")
        {
            anim = GameObject.Find("cu_puppy_shiba_a").GetComponent<Animator>();
            anim.runtimeAnimatorController = Resources.Load("AnimationController/PetEatController") as RuntimeAnimatorController;
            isEating = true;
            eatTime = 160;

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
                petPos.petLastPosX = 0f;
                petPos.petLastPosY = 0f;
                petPos.petPosX = this.transform.position.x;
                petPos.petPosY = this.transform.position.z;
                petPos.tileX = tileX;
                petPos.tileY = tileY;
                petPos.petState = "eatFood";
                petPos.speed = speed;

                string requestJson = JsonUtility.ToJson(petPos);
                AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);
            }
        }

        if (col.gameObject.tag == "Ball")
        {
            ball.transform.parent = this.transform;

            ballDeliver = true;

            var mapScript = mapController.GetComponent<MapController>();
            int tileX = mapScript.tileX;
            int tileY = mapScript.tileY;
            bool readyToSend = mapScript.okToSentPetPos;

            if (readyToSend)
            {
                // movement
                timeStartMove = DateTime.Now.Ticks.ToString();

                UpdatePetPos petPos = new UpdatePetPos();
                petPos.type = "listplayer";
                petPos.username = PlayerPrefs.GetString("username");
                petPos.timeStartMove = timeStartMove;
                petPos.petLastPosX = 0f;
                petPos.petLastPosY = 0f;
                petPos.petPosX = Camera.main.transform.position.x;
                petPos.petPosY = Camera.main.transform.position.z;
                petPos.tileX = tileX;
                petPos.tileY = tileY;
                petPos.petState = "walkbringball";
                petPos.speed = speed;

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
    }

    void ShowAction()
    {
        actionPanel.gameObject.SetActive(!actionActive);
        actionActive = !actionActive;
    }

    void PlayBall()
    {
        if (!ballThrown)
        {
            if (!ballPlay)
            {

                ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ball.tag = "Ball";
                ball.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                float pos = (Camera.main.nearClipPlane + 2f);
                ball.transform.position = Camera.main.transform.position + Camera.main.transform.forward * pos;

                Rigidbody ballRigibody = ball.AddComponent<Rigidbody>();
                ballRigibody.mass = 1;
                ballRigibody.isKinematic = true;

                ballPlay = true;

                var mapScript = mapController.GetComponent<MapController>();
                int tileX = mapScript.tileX;
                int tileY = mapScript.tileY;
                bool readyToSend = mapScript.okToSentPetPos;

                if (readyToSend)
                {
                    UpdateBallState updateBall = new UpdateBallState();
                    updateBall.type = "updateBall";
                    updateBall.username = PlayerPrefs.GetString("username");
                    updateBall.tileX = tileX;
                    updateBall.tileY = tileY;
                    updateBall.ballPosX = Camera.main.transform.position.x;
                    updateBall.ballPosY = 10.0f;
                    updateBall.ballPosZ = Camera.main.transform.position.z;
                    updateBall.ballState = "live";

                    string requestJson = JsonUtility.ToJson(updateBall);
                    AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);
                }
            }
            else
            {
                Destroy(GameObject.FindGameObjectWithTag("Ball"));
                ballPlay = false;

                var mapScript = mapController.GetComponent<MapController>();
                int tileX = mapScript.tileX;
                int tileY = mapScript.tileY;
                bool readyToSend = mapScript.okToSentPetPos;

                if (readyToSend)
                {
                    UpdateBallState updateBall = new UpdateBallState();
                    updateBall.type = "updateBall";
                    updateBall.username = PlayerPrefs.GetString("username");
                    updateBall.tileX = tileX;
                    updateBall.tileY = tileY;
                    updateBall.ballPosX = Camera.main.transform.position.x;
                    updateBall.ballPosY = 10.0f;
                    updateBall.ballPosZ = Camera.main.transform.position.z;
                    updateBall.ballState = "none";

                    string requestJson = JsonUtility.ToJson(updateBall);
                    AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);
                }
            }
        }  
    }

    void MoveBallMatchCameraRotation()
    {
        if (!ballThrown)
        {
            GameObject ball = GameObject.FindGameObjectWithTag("Ball");
            float pos = (Camera.main.nearClipPlane + 2f);
            ball.transform.position = Camera.main.transform.position + Camera.main.transform.forward * pos;
        }
    }

    void ThrowBall()
    {
        if (!ballThrown)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    ball.GetComponent<Rigidbody>().isKinematic = false;
                    ball.GetComponent<Rigidbody>().velocity = Camera.main.transform.forward * 5;

                    ballThrown = true;

                    var mapScript = mapController.GetComponent<MapController>();
                    int tileX = mapScript.tileX;
                    int tileY = mapScript.tileY;
                    bool readyToSend = mapScript.okToSentPetPos;

                    if (readyToSend)
                    {
                        UpdateBallState updateBall = new UpdateBallState();
                        updateBall.type = "updateBall";
                        updateBall.username = PlayerPrefs.GetString("username");
                        updateBall.tileX = tileX;
                        updateBall.tileY = tileY;
                        updateBall.ballPosX = Camera.main.transform.forward.x;
                        updateBall.ballPosY = Camera.main.transform.forward.y;
                        updateBall.ballPosZ = Camera.main.transform.forward.z;
                        updateBall.ballState = "throw";

                        string requestJson = JsonUtility.ToJson(updateBall);
                        AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);
                    }
                }
            }
        }
    }

    void SimulateProjectile()
    {
        if (ball.transform.position.y <= 0.0f && !ballInGround)
        {
            ball.GetComponent<Rigidbody>().isKinematic = true;
            ball.transform.position = new Vector3(ball.transform.position.x, 0.0f, ball.transform.position.z);

            ballInGround = true;

            petCalled = false;

            var mapScript = mapController.GetComponent<MapController>();
            int tileX = mapScript.tileX;
            int tileY = mapScript.tileY;
            bool readyToSend = mapScript.okToSentPetPos;

            if (readyToSend)
            {
                // ball
                UpdateBallState updateBall = new UpdateBallState();
                updateBall.type = "updateBall";
                updateBall.username = PlayerPrefs.GetString("username");
                updateBall.tileX = tileX;
                updateBall.tileY = tileY;
                updateBall.ballPosX = ball.transform.position.x;
                updateBall.ballPosY = 0.0f;
                updateBall.ballPosZ = ball.transform.position.z;
                updateBall.ballState = "inground";

                string requestJson = JsonUtility.ToJson(updateBall);
                AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);

                // movement
                timeStartMove = DateTime.Now.Ticks.ToString();

                UpdatePetPos petPos = new UpdatePetPos();
                petPos.type = "listplayer";
                petPos.username = PlayerPrefs.GetString("username");
                petPos.timeStartMove = timeStartMove;
                petPos.petLastPosX = transform.position.x;
                petPos.petLastPosY = transform.position.z;
                petPos.petPosX = ball.transform.position.x;
                petPos.petPosY = ball.transform.position.z;
                petPos.tileX = tileX;
                petPos.tileY = tileY;
                petPos.petState = "walktoball";
                petPos.speed = speed;

                string requestJson2 = JsonUtility.ToJson(petPos);
                AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson2);
            }
        }
    }

    void MovePlay()
    {
        if (ballInGround)
        {
            Vector3 lookpos = ball.transform.position - this.transform.position;
            lookpos.y = 0;
            if (lookpos != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(lookpos);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime * 2.0f);
            }

            this.transform.position = Vector3.MoveTowards(this.transform.position, ball.transform.position, Time.deltaTime * speed);
        }
    }

    void MoveDeliver()
    {
        if (ballDeliver)
        {
            Vector3 camPos = new Vector3(Camera.main.transform.position.x, 0.0f, Camera.main.transform.position.z);
            float distance = Vector3.Distance(this.transform.position, camPos);
            if (distance > 0.1f)
            {
                Vector3 lookpos = camPos - this.transform.position;
                lookpos.y = 0;
                if (lookpos != Vector3.zero)
                {
                    var rotation = Quaternion.LookRotation(lookpos);
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime * 2.0f);
                }

                this.transform.position = Vector3.MoveTowards(this.transform.position, camPos, Time.deltaTime * speed);
            }else
            {
                ball.transform.parent = null;
                Destroy(GameObject.FindGameObjectWithTag("Ball"));
                ballDeliver = false;
                ballInGround = false;
                ballPlay = false;
                ballThrown = false;

                targetMove = transform.position;

                int curAgility = PlayerPrefs.GetInt("Walk");
                PlayerPrefs.SetInt("Walk", curAgility + 2);

                // update ball
                var mapScript = mapController.GetComponent<MapController>();
                int tileX = mapScript.tileX;
                int tileY = mapScript.tileY;
                bool readyToSend = mapScript.okToSentPetPos;

                if (readyToSend)
                {
                    UpdateBallState updateBall = new UpdateBallState();
                    updateBall.type = "updateBall";
                    updateBall.username = PlayerPrefs.GetString("username");
                    updateBall.tileX = tileX;
                    updateBall.tileY = tileY;
                    updateBall.ballPosX = Camera.main.transform.position.x;
                    updateBall.ballPosY = 0.0f;
                    updateBall.ballPosZ = Camera.main.transform.position.z;
                    updateBall.ballState = "none";

                    string requestJson = JsonUtility.ToJson(updateBall);
                    AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);
                }
            }
        }
    }

    void CallPet()
    {
        if (!foodEnabled && !ballInGround)
        {
            petCalled = true;

            targetMove = new Vector3(Camera.main.transform.position.x, 0.0f, Camera.main.transform.position.z);

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
                petPos.petLastPosX = transform.position.x;
                petPos.petLastPosY = transform.position.z;
                petPos.petPosX = targetMove.x;
                petPos.petPosY = targetMove.z;
                petPos.tileX = tileX;
                petPos.tileY = tileY;
                petPos.petState = "call";
                petPos.speed = speed;

                string requestJson = JsonUtility.ToJson(petPos);
                AmqpClient.Publish(AmqpController.amqpControl.requestExchangeName, AmqpController.amqpControl.requestRoutingKey, requestJson);

            }
        }
    }

    void CallPetMove()
    {
        Vector3 camPos = new Vector3(Camera.main.transform.position.x, 0.0f, Camera.main.transform.position.z);
        float distance = Vector3.Distance(this.transform.position, camPos);
        if (distance > 0.1f)
        {
            Vector3 lookpos = camPos - this.transform.position;
            lookpos.y = 0;
            if (lookpos != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(lookpos);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime * 2.0f);
            }

            this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(Camera.main.transform.position.x, 0.0f, Camera.main.transform.position.z), Time.deltaTime * speed);
        }
        else
        {
            petCalled = false;
            targetMove = transform.position;
        }
    }

    [Serializable]
    class UpdatePetPos
    {
        public string type;
        public string username;
        public string timeStartMove;
        public float petLastPosX;
        public float petLastPosY;
        public float petPosX;
        public float petPosY;
        public int tileX;
        public int tileY;
        public string petState;
        public float speed;
    }

    class UpdateBallState
    {
        public string type;
        public string username;
        public int tileX;
        public int tileY;
        public float ballPosX;
        public float ballPosY;
        public float ballPosZ;
        public string ballState;
    }
}
