using UnityEngine;
using System.Collections;

// this component will track collisions with other GameObjects with a certain tag
// 

public class BoneCatcher : MonoBehaviour 
{

    public Care FunCare;
    private int bonesCaught = 0;

    public void ResetBonesCaught()
    {
        bonesCaught = 0;
    }

    public int GetBonesCaught()
    {
        return bonesCaught;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Bone")
        {
            Debug.Log("Bone Caught!!!");
            SaveManager.Instance.Bones ++;
            SaveManager.Instance.XP++;
            Destroy(col.gameObject);
            bonesCaught++;

            FunCare.AddCare();
        }

    }
}
