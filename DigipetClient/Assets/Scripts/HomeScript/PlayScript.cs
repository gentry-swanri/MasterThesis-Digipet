using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayScript : MonoBehaviour {

    public Button play;

	// Use this for initialization
	void Start () {
        play = this.GetComponent<Button>();
        play.onClick.AddListener(PlayWithPet);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayWithPet()
    {
        SceneManager.LoadScene("VirtualPlayScene");
    }
}
