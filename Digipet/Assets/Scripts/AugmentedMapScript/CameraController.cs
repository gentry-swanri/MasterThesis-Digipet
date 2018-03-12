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
        this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Input.gyro.attitude * rotQuat, Time.deltaTime * 2.0f);
    }

}
