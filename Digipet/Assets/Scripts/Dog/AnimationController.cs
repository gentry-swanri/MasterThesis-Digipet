using UnityEngine;
using System.Collections;

// This component will switch animations on our character
// It will also play barking sound when our dog character should bark

public class AnimationController : MonoBehaviour {

    public Transform VisualChar;

    private Animator anim;
    private AudioSource source;
    private Training training;
    private TouchController controller;

    private bool facingLeft = true;

    private bool walking = false;

    // Use this for initialization
    void Awake () 
    {
        anim = GetComponentInChildren<Animator> ();
        source = GetComponent<AudioSource> ();
        training = GetComponent<Training>();
        controller = GetComponent<TouchController>();
    }
        
    void Update()
    {
        walking = controller.Walking;

        // play barking sound
        if (training.barking && !walking &&!source.isPlaying)
            source.Play ();
        else if ((!training.barking || walking) && source.isPlaying)
            source.Stop ();

        anim.SetBool ("Walking", walking);
        anim.SetBool ("Barking", training.barking);
        anim.SetBool ("Standing", !training.sitting);
        anim.SetBool ("Looking", training.looking);
    }

    /*void Update()
    {
        walking = controller.Walking;

        bool barking = false;
        bool standing = true;
        bool looking = false;

        if (Input.GetKey(KeyCode.B))
            barking = true;

        if (Input.GetKey(KeyCode.L))
            looking = true;

        if (Input.GetKey(KeyCode.S))
            standing = false;


        anim.SetBool ("Walking", walking);
        anim.SetBool ("Barking", barking);
        anim.SetBool ("Standing", standing);
        anim.SetBool ("Looking", looking);
    }*/
        

}

