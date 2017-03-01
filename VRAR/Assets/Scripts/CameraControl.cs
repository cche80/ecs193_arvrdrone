using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class CameraControl : MonoBehaviour {
    #region GPS Variables
    public string DEVICE;
    public long TIME_STAMP;
    public bool STATUS;
    public float LAT;
    public float LON;
    public float ALT;
    public int SAT;
    public int PREC;
    public long CHARS;
    public int SENTENCES;
    public int CSUM_ERR;
    public float iniLAT;
    public float iniLON;
    public float iniALT;

    public bool firstGPSflag;
    public static string COMPort = "COM3";
    public static int BAUDRate = 115200;

    SerialPort streamCable = new SerialPort(COMPort, BAUDRate); //Set the port (com4) and the baud rate (9600, is standard on most devices)
    #endregion

    #region Player Control Variables
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;

    public int playerControlSpeed = 10;
    public bool playerControlFlag;
    #endregion

    // Use this for initialization
    void Start() {
        Debug.Log("Start  ");
        firstGPSflag = true;
        playerControlFlag = true;
        streamCable.Open(); //Open the Serial Stream.
    }

    // Update is called once per frame
    void Update() {
        if (!playerControlFlag) {
            readGPS();
            move();
            if (Input.GetKeyDown(KeyCode.C)) {
                playerControlFlag = true;
                LAT = 0;
                LON = 0;
                iniLAT = 0;
                iniLON = 0;
            }
        } else {
            // ccmove();
            playerControl();
            if (Input.GetKeyDown(KeyCode.C)) {
                playerControlFlag = false;
            }
        }
    }

    void readGPS() {
        if (streamCable.IsOpen) {
            string jsonString = streamCable.ReadLine();
            print(jsonString);
            try {
                JsonUtility.FromJsonOverwrite(jsonString, this);
                Debug.Log("device = " + this.DEVICE);
                Debug.Log("time_stamp = " + this.TIME_STAMP);
                Debug.Log("status = " + this.STATUS);
                Debug.Log("lat = " + this.LAT);
                Debug.Log("lon = " + this.LON);
                Debug.Log("alt = " + this.ALT);
                Debug.Log("sat = " + this.SAT);
                Debug.Log("prec = " + this.PREC);
                Debug.Log("chars = " + this.CHARS);
                Debug.Log("sentences = " + this.SENTENCES);
                Debug.Log("csum_err = " + this.CSUM_ERR);
            } catch (System.Exception) {
                Debug.Log("fail to Json.");
                throw;
            }
        } else {
            Debug.Log("No  ");
        }
    }

    void ccmove() {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded) {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void playerControl() {
        if (Input.GetKey(KeyCode.D)) {
            transform.Translate(new Vector3(playerControlSpeed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.Translate(new Vector3(-playerControlSpeed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.Q)) {
            transform.Translate(new Vector3(0, -playerControlSpeed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.E)) {
            transform.Translate(new Vector3(0, playerControlSpeed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.W)) {
            transform.Translate(new Vector3(0, 0, playerControlSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.Translate(new Vector3(0, 0, -playerControlSpeed * Time.deltaTime));
        }
    }

    void move() {
        if (firstGPSflag == true && LON != 0 && LAT != 0 && ALT != 0) {
            iniLON = LON;
            iniLAT = LAT;
            iniALT = ALT;
            firstGPSflag = false;
        } else if (LON != 0 && LAT != 0) {
            float la = (float)((LAT - 53.178469) / 0.00001 * 0.12179047095976932582726898256213);
            float lo = (float)((LON - 53.178469) / 0.00001 * 0.12179047095976932582726898256213);
            float ila = (float)((iniLAT - 53.178469) / 0.00001 * 0.12179047095976932582726898256213);
            float ilo = (float)((iniLON - 53.178469) / 0.00001 * 0.12179047095976932582726898256213);
            //transform.position = Quaternion.AngleAxis(LON-iniLON, -Vector3.up) * Quaternion.AngleAxis(LAT-iniLAT, -Vector3.right) * new Vector3(0, 0, 1);
            transform.position = new Vector3((lo - ilo), 0.5f, (la - ila));
        }
    }
}