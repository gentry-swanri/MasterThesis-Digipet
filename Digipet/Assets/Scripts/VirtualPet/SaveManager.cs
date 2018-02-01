using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// this script will manage XP points and bones (currency in our game)
// 

public class SaveManager : MonoBehaviour {

    public static SaveManager Instance;

    public Text BonesText;
    public Text XPText;

    public Image XPProgressFill;

    private int bones = 0;

    public int Bones
    {
        get
        {
            return bones;
        }
        set
        {
            bones = value;
            // Update Text
            // Debug.Log("SaveManager attached to:" + gameObject.name);
            BonesText.text = bones.ToString();
        }
    }
    private int xp = 0;
    private int nextLevelTotalXp = 20;
    private int nextLevelXpMultiplier = 2;
    private int previousLevelTotalXp = 0;
    private int currentLevel = 1;

    public int XP
    {
        get
        {
            return xp;
        }
        set
        {
            xp = value;

            // check if we reached next level
            if (xp > nextLevelTotalXp)
            {
                previousLevelTotalXp = nextLevelTotalXp;
                // update next level total
                nextLevelTotalXp*=nextLevelXpMultiplier;
                currentLevel++;
                // Call level change method
                GameObject.FindObjectOfType<LevelManager>().LevelChange(currentLevel);
            }
           
            UpdateProgressBar();

            XPText.text = "XP("+ xp.ToString() +"/"+nextLevelTotalXp.ToString()+")";
        }
    }

	// Use this for initialization
	void Awake () 
    {
        Instance = this;
        //PlayerPrefs.DeleteAll();
	}

    void Start()
    {
        LoadFromPlayerPrefs();
        // this line is for debugging
        // Bones = 1000;
        // XP = 0;
        GameObject.FindObjectOfType<LevelManager>().LoadCharLevel(currentLevel);
    }
	
    void LoadFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("NextLevelXP"))
            nextLevelTotalXp = PlayerPrefs.GetInt("NextLevelXP");

        if (PlayerPrefs.HasKey("PreviousLevelXP"))
            previousLevelTotalXp = PlayerPrefs.GetInt("PreviousLevelXP");
        
        if (PlayerPrefs.HasKey("Bones"))
            Bones = PlayerPrefs.GetInt("Bones");
        else
            Bones = 100;
        
        if (PlayerPrefs.HasKey("XP"))
            XP = PlayerPrefs.GetInt("XP");
        else
            XP = 0;

        if (PlayerPrefs.HasKey("CurrentLevel"))
            currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        GameObject.FindObjectOfType<LevelManager>().LoadCharLevel(currentLevel);
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("Bones", bones);
        PlayerPrefs.SetInt("XP", xp);
        PlayerPrefs.SetInt("NextLevelXP", nextLevelTotalXp);
        PlayerPrefs.SetInt("PreviousLevelXP", previousLevelTotalXp);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);

        PlayerPrefs.Save();
    }

    void UpdateProgressBar()
    {
        // calculate Progress
        float progress = (float)(xp - previousLevelTotalXp) /
            (float)(nextLevelTotalXp - previousLevelTotalXp);

        XPProgressFill.fillAmount = progress;
    }

    void OnApplicationQuit()
    {
        SaveData();
    }

   
}
