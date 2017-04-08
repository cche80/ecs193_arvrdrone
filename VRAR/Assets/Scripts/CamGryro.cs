using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamGryro : MonoBehaviour {
    public GameObject webPlane;
    GameObject camParent;

    // Use this for initialization
    void Start () {
        camParent = new GameObject("CamParent");
        camParent.transform.position = this.transform.position;
        this.transform.parent = camParent.transform;
        camParent.transform.Rotate(Vector3.right, 90);
        Input.gyro.enabled = true;

        WebCamTexture webCamTexture = new WebCamTexture();
        webPlane.GetComponent<MeshRenderer>().material.mainTexture = webCamTexture;
        webCamTexture.Play();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
