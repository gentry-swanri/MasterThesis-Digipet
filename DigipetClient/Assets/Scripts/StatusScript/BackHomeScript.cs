using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackHomeScript : MonoBehaviour {

    public Button home;

	// Use this for initialization
	void Start () {
        home = this.GetComponent<Button>();
        home.onClick.AddListener(BackHome);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BackHome()
    {
        SceneManager.LoadScene("HomeScene");
    }
}
