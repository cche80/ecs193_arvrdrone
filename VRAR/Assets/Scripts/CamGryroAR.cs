using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamGryroAR : MonoBehaviour {
    public GameObject webPlane;
    public int N_camera = 0;
    GameObject camParent;

    // Use this for initialization
    void Start()
    {
        /*camParent = new GameObject("CamParent");
        camParent.transform.position = this.transform.position;
        this.transform.parent = camParent.transform;
        camParent.transform.Rotate(Vector3.right, 90);
        Input.gyro.enabled = true;

        WebCamTexture webCamTexture = new WebCamTexture();
        webPlane.GetComponent<MeshRenderer>().material.mainTexture = webCamTexture;
        webCamTexture.Play();
        */
        WebCamDevice[] devices = WebCamTexture.devices;
        WebCamTexture webcamTexture = new WebCamTexture();
        Debug.Log("Number of Camera: " + devices.Length);
        if (devices.Length != 0)
        {
            webcamTexture.deviceName = devices[N_camera].name;
            webPlane.GetComponent<MeshRenderer>().material.mainTexture = webcamTexture;
            webcamTexture.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
