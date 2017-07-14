using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadGameScript : MonoBehaviour {

    public Button loadGame;

	// Use this for initialization
	void Start () {
        loadGame = this.GetComponent<Button>();
        loadGame.onClick.AddListener(LoadGameOnClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LoadGameOnClick()
    {
        Debug.Log("Load Game");

        if (DataControllerScript.dataController != null)
        {
            bool loaded = DataControllerScript.dataController.LoadData();
            if (loaded)
            {
                SceneManager.LoadScene("HomeScene");
            }else
            {
                Debug.Log("Save Data Not Found");
            }
        }else
        {
            Debug.Log("Data Controller Not Ready");
        }
    }
}
