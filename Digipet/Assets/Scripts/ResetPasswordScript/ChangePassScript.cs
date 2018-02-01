using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangePassScript : MonoBehaviour {

    Button changeBut;

	// Use this for initialization
	void Start () {
        changeBut = this.GetComponent<Button>();
        changeBut.onClick.AddListener(ChangeAction);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeAction()
    {
        SceneManager.LoadScene("ChangePassScene");
    }
}
