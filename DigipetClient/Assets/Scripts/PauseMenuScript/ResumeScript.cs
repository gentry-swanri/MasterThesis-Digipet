using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResumeScript : MonoBehaviour {

    public Button resume;

	// Use this for initialization
	void Start () {
        resume = this.GetComponent<Button>();
        resume.onClick.AddListener(resumeGame);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void resumeGame()
    {
        SceneManager.LoadScene("AugmentedMapScene");
    }
}
