using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewAccountScript : MonoBehaviour {

    Button newAccountButton;

	// Use this for initialization
	void Start () {
        newAccountButton = this.GetComponent<Button>();
        newAccountButton.onClick.AddListener(createNewAccount);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void createNewAccount()
    {
        SceneManager.LoadScene("NewGameScene");
    }
}
