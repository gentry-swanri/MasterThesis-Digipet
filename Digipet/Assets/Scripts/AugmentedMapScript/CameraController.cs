using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour {

    private GameObject camParent;
    private GameObject plane;
    private Quaternion rotQuat;
    private WebCamTexture webcamTexture;

    // Use this for initialization
    void Start()
    {
        
        camParent = new GameObject("CameraParent");
        camParent.transform.position = this.transform.position;
        this.transform.parent = camParent.transform;
        camParent.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
        rotQuat = new Quaternion(0, 0, 1, 0);
        Input.gyro.enabled = true;

        plane = GameObject.Find("Plane");
        
        float pos = (Camera.main.nearClipPlane + 500f);
        plane.transform.position = Camera.main.transform.position + Camera.main.transform.forward * pos;
        float h = (Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f) / 10.0f;
        plane.transform.localScale = new Vector3(h * Camera.main.aspect, 1.0f, h);

        if (webcamTexture == null)
        {
            webcamTexture = new WebCamTexture();
            plane.GetComponent<MeshRenderer>().material.mainTexture = webcamTexture;
        }
         
        webcamTexture.Play();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.localRotation = Input.gyro.attitude * rotQuat;
        this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Input.gyro.attitude * rotQuat, Time.deltaTime * 2.0f);
        //this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, new Quaternion(-Input.gyro.attitude.x, -Input.gyro.attitude.y, Input.gyro.attitude.z, Input.gyro.attitude.w), Time.deltaTime * 60f);
        //float tempZ = this.transform.eulerAngles.z;
        //this.transform.Rotate(0,0,-tempZ);
    }

}

//----------------------------------------------------------------------- References --------------------------------------------------------------------------

// http://answers.unity3d.com/questions/1101792/how-to-post-process-a-webcamtexture-in-realtime.html
// http://www.had2know.com/technology/rgb-to-gray-scale-converter.html
// https://gist.github.com/petecleary/b63acc149b618c0d08041e06f5bf915c => compass unity tutorial
