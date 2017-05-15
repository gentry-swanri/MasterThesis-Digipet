﻿using UnityEngine;
using System.Collections;

public class PetController : MonoBehaviour {

    private int timeBeforeRotate;
    private float speed;
    private bool foodEnabled;

    // Use this for initialization
    void Start()
    {
        timeBeforeRotate = Random.Range(10, 1000);
        speed = 0.1f;
        foodEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        MoveBehaviour();
        GiveFood();
        MoveToEat();
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
            if (Input.touchCount == 1)
            {
                GameObject food = GameObject.CreatePrimitive(PrimitiveType.Cube);
                food.tag = "Food";
                food.transform.localScale = new Vector3(1f, 1f, 1f);
                food.transform.position = new Vector3(Random.Range(-10, 10), 0.0f, Random.Range(-10, 10));
                Rigidbody foodRigibody = food.AddComponent<Rigidbody>();
                foodRigibody.mass = 1;
                foodRigibody.isKinematic = true;

                foodEnabled = true;
            }
        }
    }

    void MoveToEat()
    {
        if (foodEnabled)
        {
            GameObject food = GameObject.FindGameObjectWithTag("Food");
            this.transform.position = Vector3.MoveTowards(this.transform.position, food.transform.position, Time.deltaTime * speed);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //Debug.Log("Masuk Collision");
        if (col.gameObject.tag == "Food")
        {
            //Debug.Log("eat food");
            foodEnabled = false;
            timeBeforeRotate = -1;
            Destroy(col.gameObject);

            //if (DataController.HUNGER < 100)
            //{
            //    DataController.HUNGER++;
            //}
        }
    }
}
