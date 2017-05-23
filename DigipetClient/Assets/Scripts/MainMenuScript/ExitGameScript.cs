using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGameScript : MonoBehaviour {

    public Button exitGame;

	// Use this for initialization
	void Start () {
        exitGame = this.GetComponent<Button>();
        exitGame.onClick.AddListener(ExitGameOnClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ExitGameOnClick()
    {
        Application.Quit();
    }
}
