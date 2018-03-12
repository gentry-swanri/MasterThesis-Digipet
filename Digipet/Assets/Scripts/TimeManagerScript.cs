using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManagerScript : MonoBehaviour {

    public static TimeManagerScript timeManager;

    public DateTime startTime = DateTime.MinValue;

    void Start()
    {
        if (timeManager == null)
        {
            DontDestroyOnLoad(this.gameObject);
            timeManager = this;
        }
        else if(timeManager != this)
        {
            Destroy(this.gameObject);
        }

        startTime = DateTime.Now;
    }

    void Update()
    {
        
    }

    public int GetCurrentHourSpan(DateTime start)
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeSpan = currentTime.Subtract(start);
        return (int)timeSpan.TotalHours;
    }

    public int GetCurrentMinuteSpan(DateTime start)
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeSpan = currentTime.Subtract(start);
        return (int)timeSpan.TotalMinutes;
    }

    public int GetCurrentSecondSpan(DateTime start)
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeSpan = currentTime.Subtract(start);
        return (int)timeSpan.TotalSeconds;
    }

}
