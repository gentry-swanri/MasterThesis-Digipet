using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackScript : MonoBehaviour {

    private Button backButton;

	// Use this for initialization
	void Start () {
        backButton = this.GetComponent<Button>();
        backButton.onClick.AddListener(HandleBack);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void HandleBack()
    {
        SceneManager.LoadScene("HomeScene");
    }
}
