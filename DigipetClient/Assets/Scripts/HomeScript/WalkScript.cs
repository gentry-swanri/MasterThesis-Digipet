using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WalkScript : MonoBehaviour {

    public Button walk;

	// Use this for initialization
	void Start () {
        walk = this.GetComponent<Button>();
        walk.onClick.AddListener(WalkWithPet);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void WalkWithPet()
    {
        SceneManager.LoadScene("AugmentedMapScene");
    }
}
