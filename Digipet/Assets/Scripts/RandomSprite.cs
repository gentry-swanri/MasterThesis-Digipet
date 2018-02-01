using UnityEngine;
using System.Collections;

public class RandomSprite : MonoBehaviour {

    public Sprite[] Sprites;
    private SpriteRenderer sr;

   
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
	void Start () 
    {
        int index = Random.Range(0, Sprites.Length);
        sr.sprite = Sprites[index];
	}
	
	
}
