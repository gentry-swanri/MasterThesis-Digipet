using UnityEngine;
using System.Collections;

// This script will move and flip our character as necessary.
// Our character will be controlled by mouse clicks or by taps on the touch screen
// It also handles wall collisions.

public class TouchController : MonoBehaviour 
{
    public GameObject WalkTarget;

    public float walkSpeed;
    public float walkAccuracy;
    public Transform VisualChar;
    public SpeechBubble speechBubble;

    private Rigidbody2D rigi;

    private bool facingLeft = true;

    private bool collidingWallLeft = false;
    private bool collidingWallRight = false;

    private bool walking = false;

    public bool Walking 
    {
        get
        {
            return walking;
        }
    }
    // Use this for initialization
    void Awake () 
    {
        rigi = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        if (WalkTarget)
        {
            // there is a walk Target. Walking here
            walking = true;
            if (WalkTarget.transform.position.x < transform.position.x - walkAccuracy && !collidingWallLeft)
            {
                // walking left

                if (!facingLeft)
                    Flip();

                rigi.MovePosition(rigi.position + new Vector2(-walkSpeed * Time.fixedDeltaTime, 0f));
                //transform.position += new Vector3(-walkSpeed * Time.deltaTime, 0f, 0f);
            }
            else if (WalkTarget.transform.position.x > transform.position.x + walkAccuracy && !collidingWallRight)
            {
                // walking right

                if (facingLeft)
                    Flip();

                rigi.MovePosition(rigi.position + new Vector2(walkSpeed * Time.fixedDeltaTime, 0f));
                //transform.position += new Vector3(walkSpeed * Time.deltaTime, 0f, 0f);
            }
            else
            {
                // achieved destination
                Destroy(WalkTarget);
                walking = false;
            }

        }
            
    }
   
    void Flip()
    {
        Vector3 TempScale = VisualChar.transform.localScale;
        TempScale.x *= -1;
        VisualChar.transform.localScale = TempScale;
        facingLeft = !facingLeft;

        if (speechBubble != null)
            speechBubble.FlipBubbleToDirection(facingLeft);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == "Wall")
        {
            //Debug.Log("WalkTarget destroyed");
            //Destroy(WalkTarget);
            //walking = false;

            if (col.collider.bounds.center.x < transform.position.x)
            {
                collidingWallLeft = true;
            }
            else
            {
                collidingWallRight = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.tag == "Wall")
        {
            collidingWallLeft = false;
            collidingWallRight = false;
        }
    }


}

