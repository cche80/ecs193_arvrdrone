using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR;
using UnityEngine;
using System.IO.Ports;


public class camera_control: MonoBehaviour
{
    private const string logPrefix = "InputTrackerChecker: ";
    SerialPort stream = new SerialPort("COM5", 115200);

    [SerializeField]
    VRNode m_VRNode = VRNode.Head;

    private void Start()
    {
        StartCoroutine(EndOfFrameUpdate());
        while (!stream.IsOpen)
        {
            stream.Open();
        }
    }

    private void Update()
    {
        LogRotation("Update");
    }

    private void LateUpdate()
    {
        LogRotation("LateUpdate");
    }

    private IEnumerator EndOfFrameUpdate()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            LogRotation("EndOfFrame");
        }
    }

    private void LogRotation(string id)
    {
        var quaternion = InputTracking.GetLocalRotation(m_VRNode);
        var euler = quaternion.eulerAngles;
        //Debug.Log(string.Format("{0} {1}, ({2}) Quaternion {3} Euler {4}", logPrefix, id, m_VRNode, quaternion.ToString("F2"), euler.ToString("F2")));
        if (stream.IsOpen)
        {
            stream.WriteLine('X'+System.Convert.ToInt32(euler.x).ToString());
            stream.WriteLine('Y' + System.Convert.ToInt32(euler.y).ToString());
            Debug.Log(System.Convert.ToInt32(euler.x).ToString()+' '+ System.Convert.ToInt32(euler.y).ToString());
            //Debug.Log(euler.ToString());
        }
    }
}
