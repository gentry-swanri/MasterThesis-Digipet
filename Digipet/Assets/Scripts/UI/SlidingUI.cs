using UnityEngine;
using System.Collections;

public enum SlidingFromThe {Top, Bottom, Left, Right}

// Attach this component to the object that is going to slide
// This object should be a child of an object with a UI Mask
// The Hierarchy should look like
// Parent (GameObject with UI Mask and an Image component)
//      > SlidingObject (attach SlidingUI script to this GameObject)
// SlidingObject will be automatically set to stretch and fit its parent game object.

public class SlidingUI : MonoBehaviour {

	// direction to slide from
    public SlidingFromThe ElementPosition;
    // time of one slide
	public float SlideTime;
    // number of "keyframes" in one slide (steps in the coroutine)
	public int Keyframes;
    // if the elemetn should be initially shown or hidden
    public bool initiallyVisible;

    // if currently a slide is performed
    private bool isSliding = false;
    // backing variable for a property Visible
    private bool visible;

    // Set Visible to true to slide in and show the sliding element
    // Visible to false to slide out and hide the sliding element
    public bool Visible
    {
        get
        {
            return visible;
        }

        set
        {
            if (value != visible && !isSliding)
            {
                visible = value;
                if (visible)
                    StartCoroutine(SlideCoroutine (1f));
                else
                    StartCoroutine(SlideCoroutine (-1f));
            }
        }
    }
    // delay for coroutine step is calculated as SlideTime/Keyframes
	private float floatDelay;
	private WaitForSeconds delay;
    // dimensions of the sliding RectTransform after the Canvas is drawn and updated
	private float pixelWidth;
	private float pixelHeight;

	private RectTransform rt;
   
    // canvas group to ensure that UI is not interactable during slide
    private CanvasGroup cg;

	void Awake()
	{
        cg = GetComponent<CanvasGroup>();

		rt = GetComponent<RectTransform> ();
        // calculate delay
		floatDelay = SlideTime / Keyframes;
        // create one WaitForSeconds to use in sliding coroutine
		delay = new WaitForSeconds (floatDelay);
        // set this Gameobject to fit the mask
        StretchAndFitParent();
        visible = initiallyVisible;
	}

	void Start()
	{
        // get width and height after canvas update
        ResetWidthAndHeight();
	}

    private void StretchAndFitParent()
    {
        rt.anchorMax = new Vector2(1f, 1f);
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchoredPosition = Vector2.zero;
    }

    // this method should be called only if the dimensions of the canvas change.
    // for example, after the screen orientation is changed to get new dimensions of a sliding object
    public void ResetWidthAndHeight()
    {
        Canvas.ForceUpdateCanvases();
        pixelWidth = rt.rect.width;
        pixelHeight = rt.rect.height;
        SetVisiblePosition();
    }

    // use this coroutine if the method GetWidthAndHeight() does not work
	IEnumerator ResetWidthAndHeightCoroutine()
	{
		// disable children
		transform.GetChild(0).gameObject.SetActive (false);

		yield return new WaitForEndOfFrame ();

		pixelWidth = rt.rect.width;
		pixelHeight = rt.rect.height;

		SetVisiblePosition ();

		// enable children
		transform.GetChild(0).gameObject.SetActive (true);
	}
        
	void SetVisiblePosition()
	{
        // Set initial position according to Visible value
        if (!Visible)
            HideInstantly();
        else
            rt.anchoredPosition = Vector2.zero;
	}

    // Method to show the SlidingUI Element instantly without sliding
    public void ShowInstantly()
    {
        visible = true;
        rt.anchoredPosition = Vector2.zero;
    }

    // Method to hide the SlidingUI Element instantly without sliding
	public void HideInstantly()
	{
		Debug.Log("Hiding");
		Vector2 offset = Vector2.zero;

		switch (ElementPosition) 
		{
        case SlidingFromThe.Bottom:
			offset = new Vector2 (0f, - pixelHeight);
			break;
        case SlidingFromThe.Left:
			offset = new Vector2 (- pixelWidth, 0f);
			break;
        case SlidingFromThe.Right:
			offset = new Vector2 (pixelWidth, 0f);
			break;
        case SlidingFromThe.Top:
			offset = new Vector2 (0f, pixelHeight);
			break;
		}
        visible = false;
		rt.anchoredPosition = offset;
	}

    // Call this method to slide in or out
    public void Slide()
    {
        Visible = !Visible;
    }

    private IEnumerator SlideCoroutine(float direction)
	{
        // direction = 1f - show
        // direction = -1f - hide

        isSliding = true;
        if(cg!=null)
            cg.interactable = false;

        Vector2 StepVector = Vector2.zero;

        switch (ElementPosition) 
        {
            case SlidingFromThe.Bottom:
                 StepVector = new Vector2(0f, direction*pixelHeight/Keyframes);
                 break;
            case SlidingFromThe.Left:
                 StepVector = new Vector2(direction*pixelWidth/Keyframes, 0f);
                break;
            case SlidingFromThe.Right:
                StepVector = new Vector2(-direction*pixelWidth/Keyframes, 0f);
                break;
            case SlidingFromThe.Top:
                StepVector = new Vector2(0f, -direction*pixelHeight/Keyframes);
                break;
        }

        for (int  i= 0; i< Keyframes; i++) 
        {
            rt.anchoredPosition += StepVector;
            yield return delay;
            yield return new WaitForEndOfFrame ();
        }
            
        isSliding = false;
        if (cg!= null)
            cg.interactable = true;
	}


}
