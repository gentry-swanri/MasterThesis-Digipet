using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;


// Attach this script to any game object that you would like to click on in your scene.
// This class has 3 UnityEvents that will be visible in the inspector and let you control 
// what happens when the user clicks on this GameObject.
// While hovering over the game object you can highlight it by changing its color and scale
// HitClickableThisFrame property returns true if user clicked on any of the GameObjects
// with this script in the scene.
public class ClickableObject: MonoBehaviour,  IEventSystemHandler
{
    public static bool HitClickableThisFrame{ get; set;}
    //public string Description;
    public float HighlightScaleFactor = 1.1f;
    public Color32 HighlihtColor = Color.white;

    [SerializeField]
    public UnityEvent OnClicked = new UnityEvent();
    [SerializeField]
    public UnityEvent OnPointerEnter = new UnityEvent();
    [SerializeField]
    public UnityEvent OnPointerExit = new UnityEvent();

    SpriteRenderer sr;
    private Vector3 savedScale;

    // Use this for initialization
	void Start () 
    {
        sr = GetComponent<SpriteRenderer>();
        savedScale = transform.localScale;
	}
	
    void OnMouseEnter()
    {
        // highlight
        sr.color = HighlihtColor;
        transform.localScale = savedScale * HighlightScaleFactor;
        OnPointerEnter.Invoke();
    }

    void OnMouseExit()
    {
        // stop highlighting
        sr.color = Color.white;
        transform.localScale = savedScale;
        OnPointerExit.Invoke();
    }

    void OnMouseDown()
    {
        // invoke clicked event
        OnClicked.Invoke();
        HitClickableThisFrame = true;
    }

    void LateUpdate()
    {
        HitClickableThisFrame = false;
    }
}
