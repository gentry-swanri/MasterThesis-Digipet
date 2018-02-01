using UnityEngine;
using System.Collections;

// this script will make our character gow in size when he reaches certain level

public class LevelManager : MonoBehaviour {

    public float[] FiveScaleValues = new float[5];
    public int[] JumpLevels = new int[4];
    public GameObject VisualCharacter;

    public void LoadCharLevel(int level)
    {
        int index;
        if (level > 0 && level <= JumpLevels[0])
            index = 0;
        else if (level > JumpLevels[0] && level <= JumpLevels[1])
            index = 1;
        else if (level > JumpLevels[1] && level <= JumpLevels[2])
            index = 2;
        else if (level > JumpLevels[2] && level <= JumpLevels[3])
            index = 3;
        else
            index = 4;
    
        if (VisualCharacter.transform.localScale.x >0)
            VisualCharacter.transform.localScale = new Vector3(FiveScaleValues[index], FiveScaleValues[index], FiveScaleValues[index]);
        else
            VisualCharacter.transform.localScale = new Vector3(-FiveScaleValues[index], FiveScaleValues[index], FiveScaleValues[index]);
        Debug.Log("Level+ " +level.ToString()+
            "    Loaded scale: " + FiveScaleValues[index].ToString());
    }

    public void LevelChange(int Level)
    {
        if (Level>1)
            DogTalkManager.Instance.Say("I have grown a little!!! Now I am Level " + Level.ToString(), 6f);
        LoadCharLevel(Level);
        Debug.Log("Growing");
    }
}
