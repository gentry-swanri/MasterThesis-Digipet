using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReturnToHome : MonoBehaviour {

    Button returnBut;

    // Use this for initialization
    void Start()
    {
        returnBut = this.GetComponent<Button>();
        returnBut.onClick.AddListener(TitleAction);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TitleAction()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainScene");
    }
}
