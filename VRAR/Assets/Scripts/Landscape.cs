using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType { none, grass, sand, snow, cloud, maxBlock };

public class Block {
    public BlockType _type;
    public bool _vis;
    public GameObject _block;

    public Block(BlockType type, bool vis, GameObject block) {
        _type = type;
        _vis = vis;
        _block = block;
    }
}

public class Landscape : MonoBehaviour {
    // VR Camera
    public Camera _CameraFacing;
    // x
    public static int _width = 150;
    // y
    public static int _height = 150 + 100;
    // z
    public static int _depth = 150;

    public static Camera main;

    // PerlinNoise Parameters
    public int _heightScale = 20;
    public float _detailScale = 25.0f;
    public int _heightOffset = 100;

    // GameObjects
    public GameObject _grassBlock;
    public GameObject _sandBlock;
    public GameObject _snowBlock;
    public GameObject _cloudBlock;

    Block[,,] worldBlocks = new Block[_width, _height, _depth];

    // Use this for initialization
    void Start() {
        int seed = (int)Network.time * 10;
        for (int z = 0; z < _depth; z++) {
            for (int x = 0; x < _width; x++) {
                // If you want just a flat plane
                 int y = 0;
                // If you want regular randomness:
                // int y = (int)Random.Range(0, 10);
                // If you want smooth randomness:
                //int y = (int)(Mathf.PerlinNoise((x + seed) / _detailScale, (z + seed) / _detailScale) * _heightScale) + _heightOffset;
                Vector3 blockPos = new Vector3(x, y, z);

                CreateBlock(y, blockPos, true);
                //while (y > 0) {
                //    y--;
                    // blockPos = new Vector3(x, y, z);
                //    blockPos.y = y;
                //    CreateBlock(y, blockPos, false);
                //}
            }
        }

        CreateCloud(20, 100);
    }

    void CreateBlock(int y, Vector3 blockPos, bool create) {
        if (blockPos.x < 0 || blockPos.x >= _width || blockPos.y < 0 || blockPos.y >= _height || blockPos.z < 0 || blockPos.x >= _depth) return;
        GameObject newBlock = null;

        if (y > 15 + _heightOffset) {
            if (create)
                newBlock = (GameObject)Instantiate(_snowBlock, blockPos, Quaternion.identity);
            worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(BlockType.snow, create, newBlock);
        } else if (y > 0) {
            if (create)
                newBlock = (GameObject)Instantiate(_grassBlock, blockPos, Quaternion.identity);
            worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(BlockType.grass, create, newBlock);
        } else {
            if (create)
                newBlock = (GameObject)Instantiate(_sandBlock, blockPos, Quaternion.identity);
            worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(BlockType.sand, create, newBlock);
        }
    }

    void CreateCloud(int numClouds, int cSize) {
        for (int i = 0; i < numClouds; i++) {
            int xpos = Random.Range(0, _width - 1);
            int zpos = Random.Range(0, _depth - 1);
            for (int j = 0; j < cSize; j++) {
                Vector3 blockPos = new Vector3(xpos, _height - 1, zpos);
                GameObject newBlock = (GameObject)Instantiate(_cloudBlock, blockPos, Quaternion.identity);
                // Debug.Log(blockPos);
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(BlockType.cloud, true, newBlock);
                xpos += Random.Range(-1, 2);
                zpos += Random.Range(-1, 2);
                if (xpos < 0 || xpos >= _width) xpos = _width / 2;
                if (zpos < 0 || zpos >= _depth) zpos = _depth / 2;
            }
        }
    }

    void DrawBlock(Vector3 blockPos) {
        //
        // Prevent drawing outside of the permitted space
        //
        if (blockPos.x < 0 || blockPos.x >= _width || blockPos.y < 0 || blockPos.y >= _height || blockPos.z < 0 || blockPos.z >= _depth) return;

        if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] == null) return;

        if (!worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._vis) {
            GameObject newBlock = null;
            worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._vis = true;
            if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._type == BlockType.grass) {
                newBlock = (GameObject)Instantiate(_grassBlock, blockPos, Quaternion.identity);
            } else if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._type == BlockType.sand) {
                newBlock = (GameObject)Instantiate(_sandBlock, blockPos, Quaternion.identity);
            } else if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._type == BlockType.snow) {
                newBlock = (GameObject)Instantiate(_snowBlock, blockPos, Quaternion.identity);
            } else {
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._vis = false;
            }

            if (newBlock != null)
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._block = newBlock;
        }
    }

    int NeighbourCount(Vector3 blockPos) {
        int nCount = 0;
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
                for (int z = -1; z <= 1; z++) {
                    if (!(x == 0 && y == 0 && z == 0)) {
                        if (worldBlocks[(int)blockPos.x + x, (int)blockPos.y + y, (int)blockPos.z + z] != null)
                            nCount++;
                    }
                }
        return (nCount);
    }

    void CheckObscuredNeighbours(Vector3 blockPos) {
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
                for (int z = -1; z <= 1; z++) {
                    if (!(x == 0 && y == 0 && z == 0)) {
                        Vector3 neighbour = new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z);

                        //if it is outside the map
                        if (neighbour.x < 1 || neighbour.x > _width - 2 ||
                            neighbour.y < 1 || neighbour.y > _height - 2 ||
                            neighbour.z < 1 || neighbour.z > _depth - 2) continue;


                        if (worldBlocks[(int)neighbour.x, (int)neighbour.y, (int)neighbour.z] != null) {
                            if (NeighbourCount(neighbour) == 26) {
                                Destroy(worldBlocks[(int)neighbour.x, (int)neighbour.y, (int)neighbour.z]._block);
                                worldBlocks[(int)neighbour.x, (int)neighbour.y, (int)neighbour.z] = null;
                            }
                        }
                    }
                }
    }
    // Update is called once per frame
    void Update() {

        #region Screen Version
        /*
        if (Input.GetMouseButtonDown(0)) {
            // Debug.Log("Left Botton Detected");
            RaycastHit hit;
            // Ray ray = Camera.allCameras[0].ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            // Debug.Log(ray);
            if (Physics.Raycast(ray, out hit, 5000.0f)) {
                Vector3 blockPos = hit.transform.position;

                // this is the bottom block. Don't delete it!!!!
                if ((int)blockPos.y == 0) return;

                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;

                Destroy(hit.transform.gameObject);

                //
                // Show hidden structure underneath
                // Only instantiate hidden blocks when they are on demand!
                //
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        for (int z = -1; z <= 1; z++) {
                            if (!(x == 0 && y == 0 && z == 0)) {
                                Vector3 neighbour = new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z);
                                DrawBlock(neighbour);
                            }
                        }
                    }
                }
            }
        } else if (Input.GetMouseButtonDown(1)) {
            // Debug.Log("Right Botton Detected");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            // Debug.Log(ray.direction);
            if (Physics.Raycast(ray, out hit, 5000.0f)) {
                Vector3 blockPos = hit.transform.position;
                Vector3 hitVector = blockPos - hit.point;

                hitVector.x = Mathf.Abs(hitVector.x);
                hitVector.y = Mathf.Abs(hitVector.y);
                hitVector.z = Mathf.Abs(hitVector.z);

                // Debug.Log(ray.direction);

                if (hitVector.x > hitVector.z && hitVector.x > hitVector.y) {
                    // blockPos.x -= (int)Mathf.RoundToInt(ray.direction.x);
                    if (ray.direction.x > 0) {
                        blockPos.x -= 1;
                    } else {
                        blockPos.x += 1;
                    }
                } else if (hitVector.y > hitVector.x && hitVector.y > hitVector.z) {
                    // blockPos.y -= (int)Mathf.RoundToInt(ray.direction.y);
                    if (ray.direction.y > 0) {
                        blockPos.y -= 1;
                    } else {
                        blockPos.y += 1;
                    }
                } else {
                    // blockPos.z -= (int)Mathf.RoundToInt(ray.direction.z);
                    if (ray.direction.z > 0) {
                        blockPos.z -= 1;
                    } else {
                        blockPos.z += 1;
                    }
                }


                CreateBlock((int)blockPos.y, blockPos, true);
                CheckObscuredNeighbours(blockPos);
            }
        }
        */
        #endregion

        #region VR Single Reticle Version
        /*
        if (Input.GetMouseButtonDown(0) || Input.GetButton("Xbox_L1"))
        {
            RaycastHit hit;
            // Ray ray = Camera.allCameras[0].ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            // Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            // Debug.Log(ray);
            if (Physics.Raycast(new Ray(_CameraFacing.transform.position,
                                     _CameraFacing.transform.rotation * Vector3.forward),
                                     out hit, 5000.0f))
            {
                Vector3 blockPos = hit.transform.position;

                // this is the bottom block. Don't delete it!!!!
                if ((int)blockPos.y == 0) return;

                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;

                Destroy(hit.transform.gameObject);

                //
                // Show hidden structure underneath
                // Only instantiate hidden blocks when they are on demand!
                //
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int z = -1; z <= 1; z++)
                        {
                            if (!(x == 0 && y == 0 && z == 0))
                            {
                                Vector3 neighbour = new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z);
                                DrawBlock(neighbour);
                            }
                        }
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1) || Input.GetButton("Xbox_R1"))
        {
            // Debug.Log("Right Botton Detected");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            // Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            // Debug.Log(ray.direction);
            if (Physics.Raycast(new Ray(_CameraFacing.transform.position,
                                     _CameraFacing.transform.rotation * Vector3.forward),
                                     out hit, 5000.0f))
            {
                Vector3 blockPos = hit.transform.position;
                Vector3 hitVector = blockPos - hit.point;

                hitVector.x = Mathf.Abs(hitVector.x);
                hitVector.y = Mathf.Abs(hitVector.y);
                hitVector.z = Mathf.Abs(hitVector.z);

                // Debug.Log(ray.direction);

                if (hitVector.x > hitVector.z && hitVector.x > hitVector.y)
                {
                    // blockPos.x -= (int)Mathf.RoundToInt(ray.direction.x);
                    if (ray.direction.x > 0)
                    {
                        blockPos.x -= 1;
                    }
                    else
                    {
                        blockPos.x += 1;
                    }
                }
                else if (hitVector.y > hitVector.x && hitVector.y > hitVector.z)
                {
                    // blockPos.y -= (int)Mathf.RoundToInt(ray.direction.y);
                    if (ray.direction.y > 0)
                    {
                        blockPos.y -= 1;
                    }
                    else
                    {
                        blockPos.y += 1;
                    }
                }
                else
                {
                    // blockPos.z -= (int)Mathf.RoundToInt(ray.direction.z);
                    if (ray.direction.z > 0)
                    {
                        blockPos.z -= 1;
                    }
                    else
                    {
                        blockPos.z += 1;
                    }
                }


                CreateBlock((int)blockPos.y, blockPos, true);
                CheckObscuredNeighbours(blockPos);
            }
        }
        */
        #endregion

        #region VR Duo Reticle Version
        // Debug.Log(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger));
        // Debug.Log(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch));
        // Debug.Log();
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5f)
        {
            RaycastHit hit;
            // Ray ray = Camera.allCameras[0].ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            // Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            // Debug.Log(ray);
            if (Physics.Raycast(new Ray(_CameraFacing.transform.position + OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch),
                                     OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) * Vector3.forward),
                                     out hit, 5000.0f))
            {
                Vector3 blockPos = hit.transform.position;

                // this is the bottom block. Don't delete it!!!!
                if ((int)blockPos.y == 0) return;

                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;

                Destroy(hit.transform.gameObject);

                //
                // Show hidden structure underneath
                // Only instantiate hidden blocks when they are on demand!
                //
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int z = -1; z <= 1; z++)
                        {
                            if (!(x == 0 && y == 0 && z == 0))
                            {
                                Vector3 neighbour = new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z);
                                DrawBlock(neighbour);
                            }
                        }
                    }
                }
            }
        }
        else if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.5f)
        {
            // Debug.Log("Right Botton Detected");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            // Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            // Debug.Log(ray.direction);
            if (Physics.Raycast(new Ray(_CameraFacing.transform.position + OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch),
                                     OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) * Vector3.forward),
                                     out hit, 5000.0f))
            {
                Vector3 blockPos = hit.transform.position;
                Vector3 hitVector = blockPos - hit.point;

                hitVector.x = Mathf.Abs(hitVector.x);
                hitVector.y = Mathf.Abs(hitVector.y);
                hitVector.z = Mathf.Abs(hitVector.z);

                // Debug.Log(ray.direction);

                if (hitVector.x > hitVector.z && hitVector.x > hitVector.y)
                {
                    // blockPos.x -= (int)Mathf.RoundToInt(ray.direction.x);
                    if (ray.direction.x > 0)
                    {
                        blockPos.x -= 1;
                    }
                    else
                    {
                        blockPos.x += 1;
                    }
                }
                else if (hitVector.y > hitVector.x && hitVector.y > hitVector.z)
                {
                    // blockPos.y -= (int)Mathf.RoundToInt(ray.direction.y);
                    if (ray.direction.y > 0)
                    {
                        blockPos.y -= 1;
                    }
                    else
                    {
                        blockPos.y += 1;
                    }
                }
                else
                {
                    // blockPos.z -= (int)Mathf.RoundToInt(ray.direction.z);
                    if (ray.direction.z > 0)
                    {
                        blockPos.z -= 1;
                    }
                    else
                    {
                        blockPos.z += 1;
                    }
                }


                CreateBlock((int)blockPos.y, blockPos, true);
                CheckObscuredNeighbours(blockPos);
            }
        }
        #endregion
    }
}
