using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManagerScript : MonoBehaviour {

    public static TimeManagerScript timeManager;

    public DateTime startTime = DateTime.MinValue;
    private bool timeChecked;

    void Start()
    {
        if (timeManager == null)
        {
            DontDestroyOnLoad(this.gameObject);
            timeManager = this;

            timeChecked = false;
            if (startTime == DateTime.MinValue)
            {
                startTime = DateTime.Now;
            }
        }
        else if(timeManager != this)
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        int hourSpan = this.GetCurrentHourSpan();
        if (!timeChecked && hourSpan >= 1)
        {
            DataControllerScript.dataController.ReduceEnergy(0.24f);
            DataControllerScript.dataController.ReduceHygiene(0.24f);
            DataControllerScript.dataController.ReduceHunger(0.24f);
            DataControllerScript.dataController.ReduceFun(0.24f);
            DataControllerScript.dataController.ReduceEnvironment(0.24f);

            startTime = DateTime.Now;

            timeChecked = true;
        }

        if (hourSpan < 1)
        {
            timeChecked = false;
        }
    }

    public int GetCurrentHourSpan()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeSpan = currentTime.Subtract(startTime);
        return timeSpan.Hours;
    }

    public int GetCurrentMinuteSpan()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeSpan = currentTime.Subtract(startTime);
        return timeSpan.Minutes;
    }

    public int GetCurrentSecondSpan()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeSpan = currentTime.Subtract(startTime);
        return timeSpan.Seconds;
    }

}
