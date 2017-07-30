using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    private GameObject camParent;
    private GameObject plane;
    private Quaternion rotQuat;
    private WebCamTexture webcamTexture;

    //private Color32[] data;
    //private Texture2D texture;

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

        float pos = (Camera.main.nearClipPlane + this.transform.position.y);
        plane.transform.position = Camera.main.transform.position + Camera.main.transform.forward * pos;
        float h = (Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f) / 10.0f;
        plane.transform.localScale = new Vector3(h * Camera.main.aspect, 1.0f, h);

        webcamTexture = new WebCamTexture();
        plane.GetComponent<MeshRenderer>().material.mainTexture = webcamTexture;
        webcamTexture.Play();

        //data = new Color32[webcamTexture.width * webcamTexture.height];
        //texture = new Texture2D(webcamTexture.width, webcamTexture.height);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localRotation = Input.gyro.attitude * rotQuat;

        //if (data != null)
        //{
        //    webcamTexture.GetPixels32(data);
        //    this.Processing();
        //}
    }

    //private void Processing()
    //{
    //    for (int i = 0; i < data.Length; i++)
    //    {
    //        double x = this.ConvertToGrayscale(data[i].r, data[i].g, data[i].b);
    //        data[i].r = (byte)x;
    //        data[i].g = (byte)x;
    //        data[i].b = (byte)x;
    //    }

    //    texture.SetPixels32(data);
    //    plane.GetComponent<Renderer>().material.mainTexture = texture;
    //    texture.Apply();
        
    //}

    //private double ConvertToGrayscale(int r, int g, int b)
    //{
    //    return 0.299 * r + 0.587 * g + 0.114 * b;
    //}
}

//----------------------------------------------------------------------- References --------------------------------------------------------------------------

// http://answers.unity3d.com/questions/1101792/how-to-post-process-a-webcamtexture-in-realtime.html
// http://www.had2know.com/technology/rgb-to-gray-scale-converter.html
