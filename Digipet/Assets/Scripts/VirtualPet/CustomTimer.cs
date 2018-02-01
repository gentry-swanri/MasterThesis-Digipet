using UnityEngine;
using System.Collections;

// a very simple countdown timer that fires an event TimerElapsed each time it gets to 0

public class CustomTimer : MonoBehaviour {

    private float interval;

    public float Interval
    {
        get
        {
            return interval;
        }
        set
        {
            interval = value;
            timeLeft = interval;
        }
    }
    private float timeLeft;

    public delegate void TimerElapsed();
    public event TimerElapsed Elapsed;


	// Update is called once per frame
	void Update () 
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            timeLeft = interval;
            Elapsed.Invoke();
        }
	}
}
