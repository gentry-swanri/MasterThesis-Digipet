using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

// this game object will drop bones for our dog to catch

public class BoneDropper : MonoBehaviour, IEventSystemHandler
{

    public GameObject ObjectToDrop;
    public BoneCatcher Catcher; 
    public float initDelay;
    public float minDelay;
    public float delayStep;
    public float aboveCameraDist;

    [SerializeField]
    public UnityEvent OnGameEnd = new UnityEvent();
    [SerializeField]
    public UnityEvent AfterDelay = new UnityEvent();

    private float delay;
    private bool gamePlaying = false;
    // Use this for initialization
	void Start () 
    {
        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1f, Camera.main.nearClipPlane))
            + new Vector3(0f, aboveCameraDist, 0f);
	}

    public void StartNewBoneDropGame()
    {
        if (!gamePlaying)
        {
            Catcher.ResetBonesCaught(); 
            delay = initDelay;
            StartCoroutine(DropBonesCoroutine());
        }

    }
	
    IEnumerator DropBonesCoroutine()
    {
        gamePlaying = true;
        while (delay > minDelay)
        {
            // get random position for drop
            Vector3 dropPosition = transform.position + new Vector3(
                                       Random.Range(-transform.localScale.x/2, transform.localScale.x/2 ), 0f, 0f);
            // create a bone at random position
            Instantiate(ObjectToDrop, dropPosition, Quaternion.identity);

            // wait for delay
            yield return new WaitForSeconds(delay);

            // make bones drop faster
            delay -= delayStep;

        }
        gamePlaying = false;

		OnGameEnd.Invoke();
        yield return new WaitForSeconds(3f);
        AfterDelay.Invoke();
        ShowEndGameMessage(5f);

    }

    private void ShowEndGameMessage(float timeToShowMessage)
    {
        // compose the messgage
        string message = "WOW!"+System.Environment.NewLine + "I caught"+ 
            System.Environment.NewLine +Catcher.GetBonesCaught().ToString() + " bones!!!";
        // show the message
        DogTalkManager.Instance.Say(message, timeToShowMessage);
    }
}
