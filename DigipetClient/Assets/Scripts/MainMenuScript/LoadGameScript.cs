using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameScript : MonoBehaviour {

    public Button loadGame;

	// Use this for initialization
	void Start () {
        loadGame = this.GetComponent<Button>();
        loadGame.onClick.AddListener(LoadGameOnClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LoadGameOnClick()
    {
        Debug.Log("Load Game");
    }
}
