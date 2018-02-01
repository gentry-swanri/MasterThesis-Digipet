using UnityEngine;
using System.Collections;

public enum Side {Left, Right}

public class ScreenBoundary : MonoBehaviour 
{
    public Side SideOfBoundary;

    void Start()
    {
        if (SideOfBoundary == Side.Left)
        {
            transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.5f, Camera.main.nearClipPlane));
        }
        else
        {
            transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.5f, Camera.main.nearClipPlane));
        }
    }
	
}
