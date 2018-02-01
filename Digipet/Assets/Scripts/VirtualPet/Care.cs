using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// One of the main scripts in this Digipet virtual pet game.
// Virtual Pets demand care from players in different categories
// the value of care will decline with time until it beomes 0.
// if players do not take care of their pets you should punish them in some way.

public class Care : MonoBehaviour 
{
    public string CareName;
    public int maxPoints = 100;
    public int minusStep = 5;
    public int plusStep = 10;
    public int notifyAfterPoints = 15;
    public string Notification;
    public float DecrementTime = 500f;

    public Image ProgressBarFill;
    public Text CareNameText;
    public Text CareValueText;

    private CustomTimer timer;
    private int carePoints = 0;

    public int CarePoints
    {
        get
        {
            return carePoints;
        }
        set
        {
            // verification check
            if (value > maxPoints)
                carePoints = maxPoints;
            else if (value < 0)
            {
                carePoints = 0;
                HealthHUD.Instance.Health--;
            }
            else
                carePoints = value;

            if (carePoints < notifyAfterPoints)
                NotifyPlayer();
            // start Timer to decrement points (reset timer once you take care of your pet)
            timer.Interval = DecrementTime;

            // Update Texts and Progress bar
            ProgressBarFill.fillAmount = (float)carePoints/(float)maxPoints;
            if (carePoints < notifyAfterPoints)
                CareNameText.color = Color.red;
            else
                CareNameText.color = Color.black;

            CareValueText.text = "("+carePoints.ToString() +"/"+maxPoints.ToString()+")";
        }
    }

    void Awake()
    {
        timer = GetComponent<CustomTimer>();
    }


    void Start()
    {
        //Debug.Log(timer.gameObject.name);
        timer.Interval = DecrementTime;
        timer.Elapsed += TimeElapsed;

        LoadCare();

        CareNameText.text = CareName;
    }

    public void SaveCare()
    {
        PlayerPrefs.SetInt(CareName, carePoints);

    }

    void LoadCare()
    {
        
        if (PlayerPrefs.HasKey(CareName))
        {
            int lastSavedCare = PlayerPrefs.GetInt(CareName);
            if (SaveTimeManager.MinutesPassed > 0)
            {
                int timesToDecrement = (int)(SaveTimeManager.MinutesPassed / (double)(DecrementTime/60f));
                int TotalDecrement = minusStep * timesToDecrement;
                CarePoints = lastSavedCare - TotalDecrement;

                Debug.Log("Loaded Care: " + this.gameObject.name);
                Debug.Log("TotalDecrement: " + TotalDecrement + "        TimesToDecrement: " + timesToDecrement);

            }
            else
                CarePoints = lastSavedCare;
        }
        else
            CarePoints = maxPoints;
    }

    void OnApplicationPause(bool paused)
    {
        if (!paused)
            LoadCare();
    }

    public void AddCare()
    {
        CarePoints += plusStep;
    }

    void NotifyPlayer ()
    {
        DogTalkManager.Instance.Say(Notification, 6);
    }

    public void TimeElapsed()
    {
        CarePoints -= minusStep;
        Debug.Log("Method Time Elapsed for " + CareName + " called");
    }


}
