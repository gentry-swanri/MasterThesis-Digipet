using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatusScript : MonoBehaviour {

    public Button status;

	// Use this for initialization
	void Start () {
        status = this.GetComponent<Button>();
        status.onClick.AddListener(ShowStatus);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowStatus()
    {
        SceneManager.LoadScene("StatusScene");
    }
}
