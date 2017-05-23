using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewGameScript : MonoBehaviour {

    public Button newGame;
    

	// Use this for initialization
	void Start () {
        newGame = this.GetComponent<Button>();
        newGame.onClick.AddListener(newGameOnClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void newGameOnClick()
    {
        SceneManager.LoadScene("NewGameScene");
    }
}
