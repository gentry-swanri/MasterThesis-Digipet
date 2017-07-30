using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodScript : MonoBehaviour {

    private Button foodButton;
    private int eatTime = 160;
    private bool isEating = false;
    private Animator anim;

	// Use this for initialization
	void Start () {
        anim = GameObject.Find("cat_Idle").GetComponent<Animator>();

        foodButton = this.GetComponent<Button>();
        foodButton.onClick.AddListener(HandleFood);
	}
	
	// Update is called once per frame
	void Update () {
        if (isEating)
        {
            eatTime--;
        }

        if (eatTime <= 0)
        {
            DataControllerScript.dataController.ReduceHunger(0.24f);
            DataControllerScript.dataController.AddEnergy(0.24f);

            GameObject food = GameObject.Find("PetFood");
            Destroy(food);
            eatTime = 160;
            isEating = false;

            anim.runtimeAnimatorController = Resources.Load("AnimationController/PetIdleController") as RuntimeAnimatorController;
        }
		
	}

    void HandleFood()
    {
        eatTime = 160;
        isEating = true;

        GameObject food = GameObject.CreatePrimitive(PrimitiveType.Cube);
        food.name = "PetFood";
        food.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
        food.transform.position = new Vector3(1.0f,0,0);

        anim.runtimeAnimatorController = Resources.Load("AnimationController/PetEatController") as RuntimeAnimatorController;

    }
}
