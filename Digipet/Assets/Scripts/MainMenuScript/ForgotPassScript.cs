using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ForgotPassScript : MonoBehaviour {

    Button forgotBut;

	// Use this for initialization
	void Start () {
        forgotBut = this.GetComponent<Button>();
        forgotBut.onClick.AddListener(ForgotAction);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ForgotAction()
    {
        SceneManager.LoadScene("ForgotPassScene");
    }
}
