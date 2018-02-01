using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// this script will track our character`s health and display it on the Health HUD with hearts
// if health drops to 0 or below it will call the GameOver() method

public class HealthHUD : MonoBehaviour {

    public static HealthHUD Instance;

    public Image[] Hearts;

    public int Health
    {
        get
        { 
            return health;
        }
        set
        {
            if (value <= 0)
            {
                // game lost
                health = 0;
                GameOver();

            }
            else
            {
                if (value > maxHealth)
                {
                    Debug.Log("Health set to more then maxhealth");
                    health = maxHealth;
                }
                else
                {
                    health = value;
                }
                // show proper health
            }
            UpdateHealth();
               
        }
    }

    private int health;
    private int maxHealth;

    void Awake()
    {
        Instance = this;

        maxHealth = Hearts.Length * 2;
        if (PlayerPrefs.HasKey("Health"))
            Health = PlayerPrefs.GetInt("Health");
		else
			Health = maxHealth;
    }

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Health = 0;

        if (Input.GetKeyDown(KeyCode.W))
            Health = 1;

        if (Input.GetKeyDown(KeyCode.E))
            Health = 2;

        if (Input.GetKeyDown(KeyCode.R))
            Health = 3;

        if (Input.GetKeyDown(KeyCode.T))
            Health = 4;

        if (Input.GetKeyDown(KeyCode.Y))
            Health = 5;

        if (Input.GetKeyDown(KeyCode.U))
            Health = 6;
        
            
    }*/

    public void SaveHealth()
    {
        PlayerPrefs.SetInt("Health", health);
		PlayerPrefs.Save();
    }

    private void UpdateHealth ()
    {
        foreach (Image h in Hearts)
            h.fillAmount = 0f;

        for (int i = 0; i < health / 2; i++)
        {
            Hearts[i].fillAmount = 1f;
        }

        if (health % 2 > 0)
            Hearts[health / 2].fillAmount = 0.5f;
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        SaveHealth();
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
    }

    void OnApplicationQuit()
    {
        SaveHealth();
    }
        
}
