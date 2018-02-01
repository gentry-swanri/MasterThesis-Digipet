using UnityEngine;
using System.Collections;

public class PauseManager : MonoBehaviour 
{
    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

}
