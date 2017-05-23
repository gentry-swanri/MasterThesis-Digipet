using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGameScript : MonoBehaviour {

    Button startGame;

	// Use this for initialization
	void Start () {
        startGame = this.GetComponent<Button>();
        startGame.onClick.AddListener(StartGameOnClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void StartGameOnClick()
    {
        SceneManager.LoadScene("AugmentedMapScene");
    }
}
