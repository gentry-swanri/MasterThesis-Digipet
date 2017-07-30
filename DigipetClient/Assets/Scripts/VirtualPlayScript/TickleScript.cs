using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TickleScript : MonoBehaviour {

    private Button tickleButton;
    private Animator anim;

    private int tickleTime = 75;
    private bool isTickle = false;

	// Use this for initialization
	void Start () {
        anim = GameObject.Find("cat_Idle").GetComponent<Animator>();

        tickleButton = this.GetComponent<Button>();
        tickleButton.onClick.AddListener(HandleTickle);
	}
	
	// Update is called once per frame
	void Update () {
        
        if (isTickle)
        {
            tickleTime--;
        }

        if (tickleTime <= 0)
        {
            DataControllerScript.dataController.AddFun(0.24f);

            isTickle = false;
            tickleTime = 75;
            anim.runtimeAnimatorController = Resources.Load("AnimationController/PetIdleCOntroller") as RuntimeAnimatorController;
        }
        
	}

    void HandleTickle()
    {
        tickleTime = 75;
        isTickle = true;

        anim.runtimeAnimatorController = Resources.Load("AnimationController/PetSoundCOntroller") as RuntimeAnimatorController;
    }
}
