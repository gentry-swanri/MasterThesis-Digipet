using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// this class will display text in the speech bubble above our character

public class DogTalkManager : MonoBehaviour {

    public GameObject SpeechBubble;
    public Text SpeechText;

    public static DogTalkManager Instance;

    private bool currentlySpeaking = false;
   
    void Awake()
    {
        Instance = this;
    }

    public void SayForLongTime(string whatToSay)
    {
        Say(whatToSay, 100);
    }

    public void Say(string whatToSay, float secondsToShow)
    {
        if (secondsToShow > 0 
            && !currentlySpeaking)
            StartCoroutine(SayCoroutine(whatToSay, secondsToShow));
    }

    public void StopSpeaking()
    {
        SpeechBubble.SetActive(false);
        StopAllCoroutines();
    }

    IEnumerator SayCoroutine(string whatToSay, float secondsToShow)
    {
        currentlySpeaking = true;

        // Debug.Log("Saying something!!");
        SpeechText.text = whatToSay;
        SpeechBubble.SetActive(true);

        yield return new WaitForSeconds(secondsToShow);

        SpeechBubble.SetActive(false);

        currentlySpeaking = false;
    }
}
