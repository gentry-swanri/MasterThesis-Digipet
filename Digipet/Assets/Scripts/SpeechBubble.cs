using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour 
{
    public Sprite FacingRightBubble;
    public Sprite FacingLeftBubble;
    public Image Bubble;

    public void FlipBubbleToDirection(bool facingLeft)
    {
        if (facingLeft)
            Bubble.sprite = FacingLeftBubble;
        else
            Bubble.sprite = FacingRightBubble;
    }
}
