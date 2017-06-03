using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjMenu : MonoBehaviour {
    // Array of menu item control names.
    string[] menuOptions = new string[] { "Move", "Color", "Texture" };

    //Default selected menu item
    int selectedIndex = 1;

	// Use this for initialization
	void Start () {
        GameObject.Find(menuOptions[selectedIndex].ToString()).transform.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameObject.Find(menuOptions[selectedIndex].ToString()).transform.GetComponent<Image>().color = new Vector4(1, 1, 1, .5f);
            if (selectedIndex == 0)
            {
                selectedIndex = menuOptions.Length - 1;
            }
            else
            {
                selectedIndex -= 1;
            }
            GameObject.Find(menuOptions[selectedIndex].ToString()).transform.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            GameObject.Find(menuOptions[selectedIndex].ToString()).transform.GetComponent<Image>().color = new Vector4(1, 1, 1, .5f);
            
            if (selectedIndex == menuOptions.Length - 1)
            {
                selectedIndex = 0;
            }
            else
            {
                selectedIndex += 1;
            }
            GameObject.Find(menuOptions[selectedIndex].ToString()).transform.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);

        }

        switch (selectedIndex)
        {
            case 0:
                moveObj();
                break;
            case 1:
                changeColor();
                break;
            case 2:
                changeTexture();
                break;
            default:
                break;
        }
    }
    
    private void moveObj()
    {

    }

    private void changeColor()
    {

    }

    private void changeTexture()
    {

    }
}
