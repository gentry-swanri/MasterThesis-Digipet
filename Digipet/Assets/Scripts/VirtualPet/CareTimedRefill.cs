using UnityEngine;
using System.Collections;

// this class will restore ceratain care value on our character with time.
// for example, Sleep should be restored when the character is sleeping.

public class CareTimedRefill : MonoBehaviour {

    public bool ActivateInAwake = false;
    public bool Active { get; set;}
    public Care CareToRefill;
    public float IncrementTime;


    private CustomTimer timer;

    void Awake()
    {
        timer = GetComponent<CustomTimer>();
        if (ActivateInAwake)
            Active = true;
        else 
            Active = false;
    }


    void Start()
    {
        //Debug.Log(timer.gameObject.name);
        timer.Interval = IncrementTime;
        timer.Elapsed += TimeElapsed;
    }
	
    private void TimeElapsed()
    {
        if (Active)
            CareToRefill.AddCare();
    }
}
