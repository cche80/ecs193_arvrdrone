using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ui_update : MonoBehaviour {
	public RawImage rawimage;
    public int N_camera = 0;
	// Use this for initialization
	void Start () {
		WebCamDevice[] devices = WebCamTexture.devices;
		WebCamTexture webcamTexture = new WebCamTexture ();
		webcamTexture.deviceName=devices[N_camera].name;

		rawimage.texture = webcamTexture;
		rawimage.material.mainTexture = webcamTexture;
		webcamTexture.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
