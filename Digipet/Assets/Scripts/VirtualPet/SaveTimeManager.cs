using UnityEngine;
using System.Collections;
using System;

// this script will save the date and time of our last save action
// this is done to restore the correct values of each Care when the game is unpaused
// or when we enter the game again after closing it

public class SaveTimeManager : MonoBehaviour {

    public static SaveTimeManager Instance;
    public static TimeSpan Span;
    public static double MinutesPassed; 

	// Use this for initialization
	void Awake () 
    {
        Instance = this;
        LoadTime();
	}
	
    void OnApplicationQuit()
    {
        SaveTimeOnQuitOrPause();
    }

    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            SaveTimeOnQuitOrPause();
        }
        else
        {
            LoadTime();
        }
    }

    public void SaveTimeOnQuitOrPause()
    {
        PlayerPrefs.SetString("LastSaveTime", System.DateTime.Now.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    private void LoadTime()
    {
        if (PlayerPrefs.HasKey("LastSaveTime"))
        {
            long temp = Convert.ToInt64(PlayerPrefs.GetString("LastSaveTime"));
            DateTime lastTime = DateTime.FromBinary(temp);

            Span = System.DateTime.Now.Subtract(lastTime);
             
            MinutesPassed = Span.TotalMinutes;


        }
        else
        {
            Span = TimeSpan.MinValue;
            MinutesPassed = 0.0;
        }

        Debug.Log("LastSaveTime: " + MinutesPassed);
    }
}
