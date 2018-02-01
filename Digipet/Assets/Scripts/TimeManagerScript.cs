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

            //if (startTime == DateTime.MinValue)
            //{
                //startTime = DateTime.Now;
            //}
        }
        else if(timeManager != this)
        {
            Destroy(this.gameObject);
        }

        startTime = DateTime.Now;
    }

    void Update()
    {
        int minuteSpan = this.GetCurrentMinuteSpan(startTime);
        //if (minuteSpan >= 1 && DataControllerScript.dataController.isLogin)
        //{
            /*
            DataControllerScript.dataController.ReduceEnergy(1f);
            DataControllerScript.dataController.ReduceHygiene(1f);
            DataControllerScript.dataController.AddHunger(1f);
            DataControllerScript.dataController.ReduceFun(1f);
            DataControllerScript.dataController.ReduceEnvironment(1f);
            */

            //startTime = DateTime.Now;
        //}
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
