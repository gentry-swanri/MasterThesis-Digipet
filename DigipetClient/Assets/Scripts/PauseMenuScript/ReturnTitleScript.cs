using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReturnTitleScript : MonoBehaviour {

    public Button title;

	// Use this for initialization
	void Start () {
        title = this.GetComponent<Button>();
        title.onClick.AddListener(returnTitle);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void returnTitle()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
