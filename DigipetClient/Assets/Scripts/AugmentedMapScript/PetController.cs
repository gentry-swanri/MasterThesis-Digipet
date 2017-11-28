using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PetController : MonoBehaviour {

    private int timeBeforeRotate;
    private float speed;
    private bool foodEnabled;

    private Animator anim;
    private bool isEating;
    private int eatTime;

    // Use this for initialization
    void Start()
    {
        timeBeforeRotate = Random.Range(10, 1000);
        speed = 1.5f;
        foodEnabled = false;
        isEating = false;
    }

    // Update is called once per frame
    void Update()
    {
        MoveBehaviour();
        GiveFood();
        MoveToEat();
        Eating();
    }

    void MoveBehaviour()
    {
        if (!foodEnabled)
        {
            //Debug.Log("masuk move");
            //Debug.Log(timeBeforeRotate);
            if (timeBeforeRotate < 0)
            {
                int degreeRot = Random.Range(0, 360);
                //Debug.Log(degreeRot);
                this.transform.Rotate(0, degreeRot, 0);

                timeBeforeRotate = Random.Range(10, 1000);
            }
            else
            {
                this.transform.position += this.transform.forward * Time.deltaTime * speed;
                timeBeforeRotate--;
            }
        }
    }

    void GiveFood()
    {
        if (!foodEnabled)
        {
            if(Input.GetMouseButtonDown(0))//if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Begin)
            {
                if (!EventSystem.current.IsPointerOverGameObject()) // if(!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    GameObject food = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    food.tag = "Food";
                    food.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    food.transform.position = new Vector3(0, 0, 0);
                    //food.transform.position = new Vector3(Random.Range(-10, 10), 0.0f, Random.Range(-10, 10));
                    Rigidbody foodRigibody = food.AddComponent<Rigidbody>();
                    foodRigibody.mass = 1;
                    foodRigibody.isKinematic = true;

                    foodEnabled = true;
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
                DataControllerScript.dataController.ReduceHunger(5f);
                DataControllerScript.dataController.AddEnergy(5f);

                anim.runtimeAnimatorController = Resources.Load("AnimationController/PetWalkController") as RuntimeAnimatorController;
                eatTime = 160;
                isEating = false;

                foodEnabled = false;
                timeBeforeRotate = -1;
                Destroy(GameObject.FindGameObjectWithTag("Food"));
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //Debug.Log("Masuk Collision");
        if (col.gameObject.tag == "Food")
        {
            //Debug.Log("eat food");
            anim = GameObject.Find("cat_Walk").GetComponent<Animator>();
            anim.runtimeAnimatorController = Resources.Load("AnimationController/PetEatController") as RuntimeAnimatorController;
            isEating = true;
            eatTime = 160;

            //if (DataController.HUNGER < 100)
            //{
            //    DataController.HUNGER++;
            //}
        }
    }
}
