using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowColorMenu : MonoBehaviour {

    bool shown = false;

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            GameObject.Find("ColorMenu").GetComponent<Canvas>().enabled = !shown;
            shown = !shown;
        }
    }
}
