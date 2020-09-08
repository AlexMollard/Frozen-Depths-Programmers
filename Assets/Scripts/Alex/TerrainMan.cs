using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEditor;
using System;
using System.Security.AccessControl;

#if UNITY_EDITOR
[CustomEditor(typeof(TerrainMan))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainMan myScript = (TerrainMan)target;

        if (GUILayout.Button("Save Manager Mesh"))
        {
            myScript.SaveMesh();
        }
    }
}
#endif


public class TerrainMan : MonoBehaviour
{
    public enum spawnPrefabs
    {
        FlatAtBottom = 0,
        FlatAtTop,
        HalfFill,
        Bowl,
        XPlusWall,
        XNegWall,
        ZPlusWall,
        ZNegWall,
        PreMade
    }

    [Header("Chunk Prefab")]
    [SerializeField]
    GameObject terrainPrefab;

    List<List<List<EditableTerrain>>> terrains = new List<List<List<EditableTerrain>>>();
    List<List<List<GameObject>>> terrainsOBJS = new List<List<List<GameObject>>>();

    [Header("Player Object")]
    [Tooltip("This will be used to disable manager or meshes when player cant interact with them")]
    public GameObject player;

    [Header("Optimizations")]
    [Tooltip("Variables that will greatly effect FPS")]
    public float maxDistanceFromMesh = 100; // Distance from mesh until mesh renderer is disabled

    [Header("Total Chunks")]
    [Tooltip("This will determine how many chunks will be spawned in each axis")]
    public int terrainTotalX = 10;
    public int terrainTotalY = 10;
    public int terrainTotalZ = 10;

    [Header("Single Chunk Size")]
    [Tooltip("Terrain Width: X, TerrainHeight: Y, TerrainDepth: Z")]
    public int terrainWidth = 8;
    public int terrainHeight = 8;
    public int terrainDepth = 8;

    [Header("Geometry Settings")]
    [Tooltip("Changing these values will change how lighting and terrain manipulation effects a mesh")]
    public bool smoothTerrain = true;
    public bool flatShaded = true;

    [Header("Chunk Spawn Settings")]
    [Tooltip("Changing these values will change how each chunk will be populated")]
    public spawnPrefabs chunkPrefab = 0;

    Vector3 centerOfMeshes;
    Vector3 ChunkTotal;

    MeshRenderer[] chunkRenderers;

    void OnDrawGizmosSelected()
    {
        float halfChunkHeight = terrainHeight * 0.5f;
        float halfChunkWidth = terrainHeight * 0.5f;
        float halfChunkDepth = terrainHeight * 0.5f;

        Vector3 currentManPos = transform.position;
        Vector3 scale = new Vector3((terrainWidth - 1) * (terrainTotalX), (terrainHeight - 1) * (terrainTotalY), (terrainDepth - 1) * (terrainTotalZ));

        Vector3 centerOfMeshes = new Vector3(scale.x * 0.5f, scale.y * 0.5f, scale.z * 0.5f) + currentManPos;
        
        Gizmos.color = new Color(1, 1, 0, 0.55f);
        Gizmos.DrawCube(centerOfMeshes, scale);

        Gizmos.color = new Color(1, 0, 1, 0.65f);

        switch (chunkPrefab)
        {
            case spawnPrefabs.FlatAtBottom:
                Gizmos.DrawCube(centerOfMeshes - new Vector3(0, scale.y * 0.5f - halfChunkHeight, 0), new Vector3(scale.x, terrainHeight, scale.z));
                break;
            case spawnPrefabs.FlatAtTop:
                Gizmos.DrawCube(centerOfMeshes - new Vector3(0, halfChunkHeight, 0), new Vector3(scale.x, scale.y - terrainHeight, scale.z));
                break;
            case spawnPrefabs.HalfFill:
                Gizmos.DrawCube(centerOfMeshes - new Vector3(0, scale.y * 0.25f, 0), new Vector3(scale.x, scale.y * 0.5f, scale.z));
                break;
            case spawnPrefabs.Bowl:
                break;
            case spawnPrefabs.XPlusWall:
                break;
            case spawnPrefabs.XNegWall:
                break;
            case spawnPrefabs.ZPlusWall:
                break;
            case spawnPrefabs.ZNegWall:
                    Gizmos.DrawCube(centerOfMeshes, new Vector3(terrainWidth, terrainHeight, terrainDepth));
                break;
            case spawnPrefabs.PreMade:
                    Gizmos.DrawCube(centerOfMeshes, new Vector3(terrainWidth, terrainHeight, terrainDepth));
                break;
            default:
                break;
        }
    }



    void Start()
    {
        Vector3 currentManPos = transform.position;
        chunkRenderers = new MeshRenderer[terrainTotalX * terrainTotalY * terrainTotalZ];
        centerOfMeshes = new Vector3((terrainWidth * terrainTotalX) * 0.5f, (terrainHeight * terrainTotalY) * 0.5f, (terrainDepth * terrainTotalZ) * 0.5f) + currentManPos;
        ChunkTotal = new Vector3(terrainTotalX, terrainTotalY, terrainTotalZ);
        int i = 0;
        
        for (int x = 0; x < terrainTotalX; x++)
        {
            terrains.Add(new List<List<EditableTerrain>>());
            terrainsOBJS.Add(new List<List<GameObject>>());
            for (int y = 0; y < terrainTotalY; y++)
            {
                terrains[x].Add(new List<EditableTerrain>());
                terrainsOBJS[x].Add(new List<GameObject>());
                for (int z = 0; z < terrainTotalZ; z++)
                {
                    terrainsOBJS[x][y].Add(Instantiate(terrainPrefab, new Vector3(x * (terrainWidth - 1), y * (terrainHeight - 1), z * (terrainDepth - 1)) + currentManPos, Quaternion.identity));
                    terrainsOBJS[x][y][z].name = x + ", " + y + ", " + z;
                    terrains[x][y].Add(terrainsOBJS[x][y][z].GetComponent<EditableTerrain>());
                    terrains[x][y][z].spawnPrefab = chunkPrefab;
                    terrains[x][y][z].CreateMesh(this, new Vector3Int(x,y, z), new Vector3Int(terrainWidth - 1, terrainHeight - 1, terrainDepth - 1));
                    terrains[x][y][z].flatShaded = flatShaded;
                    terrains[x][y][z].smoothTerrain = smoothTerrain;
                    terrains[x][y][z].transform.parent = transform;

                    chunkRenderers[i] = terrains[x][y][z].GetComponent<MeshRenderer>();
                    i++;
                }
            }
        }

        AssignEdgeValues();

        bool bruh = (chunkPrefab == spawnPrefabs.PreMade) ? LoadMesh() : PopulateAllChunks();
        
        RefreshAllChunks();
    }

    bool isInDistance = true;
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
            RefreshAllChunks();

        if (isInDistance == true)
        {
            if (Vector3.Distance(player.transform.position, centerOfMeshes) > maxDistanceFromMesh)
            {
                isInDistance = false;
                for (int i = 0; i < chunkRenderers.Length; i++)
                    chunkRenderers[i].enabled = false;

                Debug.Log("Disabled renderers");
            }
        }
        else if(Vector3.Distance(player.transform.position, centerOfMeshes) <= maxDistanceFromMesh)
        {
            if (isInDistance == false)
            {
                for (int i = 0; i < chunkRenderers.Length; i++)
                    chunkRenderers[i].enabled = true;
            }

                Debug.Log("Enabled renderers");
            isInDistance = true;
        }
    }


    public void SaveMesh()
    {
        System.IO.FileStream oFileStream = null;
        oFileStream = new System.IO.FileStream(Application.dataPath + "\\TerrainSaves\\" + gameObject.name + ".txt", System.IO.FileMode.Create);


        for (int x = 0; x < terrainTotalX; x++)
        {
            for (int y = 0; y < terrainTotalY; y++)
            {
                for (int z = 0; z < terrainTotalZ; z++)
                {
                    for (int xIN = 0; xIN < terrainWidth; xIN++)
                    {
                        for (int yIN = 0; yIN < terrainHeight; yIN++)
                        {
                            for (int zIN = 0; zIN < terrainDepth; zIN++)
                            {
                                
                                oFileStream.Write(BitConverter.GetBytes(terrains[x][y][z].terrainMap[xIN, yIN, zIN].value), 0, BitConverter.GetBytes(terrains[x][y][z].terrainMap[xIN, yIN, zIN].value).Length);
                            }
                        }
                    }
                }
            }
        }
        oFileStream.Close();

    }

    public bool LoadMesh()
    {
        if (System.IO.File.Exists(Application.dataPath + "\\TerrainSaves\\" + gameObject.name + ".txt") == false)
        {
            for (int x = 0; x < terrainTotalX; x++)
            {
                for (int y = 0; y < terrainTotalY; y++)
                {
                    for (int z = 0; z < terrainTotalZ; z++)
                    {
                        terrains[x][y][z].spawnPrefab = spawnPrefabs.HalfFill;
                    }
                }
            }

            PopulateAllChunks();
            return true;
        }


        System.IO.FileStream oFileStream = null;
        oFileStream = new System.IO.FileStream(Application.dataPath + "\\TerrainSaves\\" + gameObject.name + ".txt", System.IO.FileMode.Open);


        int length = (int)oFileStream.Length;  // get file length
        byte[] buffer = new byte[length];            // create buffer
        int count;                            // actual number of bytes read
        int sum = 0;                          // total number of bytes read


        for (int x = 0; x < terrainTotalX; x++)
        {
            for (int y = 0; y < terrainTotalY; y++)
            {
                for (int z = 0; z < terrainTotalZ; z++)
                {
                    for (int xIN = 0; xIN < terrainWidth; xIN++)
                    {
                        for (int yIN = 0; yIN < terrainHeight; yIN++)
                        {
                            for (int zIN = 0; zIN < terrainDepth; zIN++)
                            {
                                terrains[x][y][z].terrainMap[xIN, yIN, zIN] = new floatMyGuy(0.0f);
                                count = oFileStream.Read(buffer, sum, length - sum);
                                sum += count;
                            } 
                        }
                    }
                }
            }
        }

        int offset = 0;
        for (int x = 0; x < terrainTotalX; x++)
        {
            for (int y = 0; y < terrainTotalY; y++)
            {
                for (int z = 0; z < terrainTotalZ; z++)
                {
                    for (int xIN = 0; xIN < terrainWidth; xIN++)
                    {
                        for (int yIN = 0; yIN < terrainHeight; yIN++)
                        {
                            for (int zIN = 0; zIN < terrainDepth; zIN++)
                            {
                                terrains[x][y][z].terrainMap[xIN, yIN, zIN].value = BitConverter.ToSingle(buffer, offset);
                                offset += 4;
                            }
                        }
                    }
                }
            }
        }
        AssignEdgeValues();



        oFileStream.Close();

        return true;
    }


    public bool PopulateAllChunks()
    {
        for (int x = 0; x < terrainTotalX; x++)
        {
            for (int y = 0; y < terrainTotalY; y++)
            {
                for (int z = 0; z < terrainTotalZ; z++)
                {
                    terrains[x][y][z].PopulateTerrainMap();
                }
            }
        }
        return true;
    }

    public void RefreshAllChunks()
    {
        for (int x = 0; x < terrainTotalX; x++)
        {
            for (int y = 0; y < terrainTotalY; y++)
            {
                for (int z = 0; z < terrainTotalZ; z++)
                {
                    terrains[x][y][z].CreateMeshData();
                }
            }
        }
    }

    public void UpdateChunk(Vector3Int index)
    {
        if (index.x >= 0 && index.x < terrainTotalX && index.y >= 0 && index.y < terrainTotalY && index.z >= 0 && index.z < terrainTotalZ)
        {
            terrains[index.x][index.y][index.z].CreateMeshData();
        }
    }

   void AssignEdgeValues()
    {
        for (int tX = 0; tX < terrainTotalX; tX++)
        {
            for (int tY = 0; tY < terrainTotalY; tY++)
            {
                for (int tZ = 0; tZ < terrainTotalZ; tZ++)
                {
                    // The corner points will be overlaping but i think this will still be fine as this should ensure that all corners are correct for neighbouring chunks
                    //tX = ->
                    //tZ = /\

                    //Top
                    if (tY + 1 < terrainTotalY)
                    {
                        // Need to grab above chunks bottom row and assign it to current chunks top row points
                        for (int x = 0; x < terrainWidth; x++)
                        {
                            for (int z = 0; z < terrainDepth; z++)
                            {
                                terrains[tX][tY][tZ].terrainMap[x, terrainHeight - 1, z] = terrains[tX][tY + 1][tZ].terrainMap[x, 0, z];
                            }
                        }
                    }


                    //Right
                    if (tX + 1 < terrainTotalX)
                    {
                        // Need to grab right chunks left row and assign it to current chunks right row points
                        for (int z = 0; z < terrainDepth; z++)
                        {
                            for (int y = 0; y < terrainHeight; y++)
                            {
                                terrains[tX][tY][tZ].terrainMap[terrainWidth - 1, y, z] = terrains[tX + 1][tY][tZ].terrainMap[0, y, z];
                            }
                        }
                    }
                    
                    //Front
                    if (tZ + 1 < terrainTotalZ)
                    {
                        // Need to grab above chunks bottom row and assign it to current chunks top row points
                        for (int x = 0; x < terrainWidth; x++)
                        {
                            for (int y = 0; y < terrainHeight; y++)
                            {
                                terrains[tX][tY][tZ].terrainMap[x, y, terrainDepth - 1] = terrains[tX][tY][tZ + 1].terrainMap[x, y, 0];
                            }
                        }
                    }


                    //Left
                    if (tX - 1 >= 0)
                    {
                        // Need to grab left chunks right row and assign it to current chunks left row points
                        for (int z = 0; z < terrainDepth; z++)
                        {
                            for (int y = 0; y < terrainHeight; y++)
                            {
                                terrains[tX][tY][tZ].terrainMap[0, y, z] = terrains[tX - 1][tY][tZ].terrainMap[terrainWidth - 1, y, z];
                            }
                        }
                    }

                    //Back
                    if (tZ - 1 >= 0)
                    {
                        // Need to grab below chunks top row and assign it to current chunks bottom row points
                        for (int x = 0; x < terrainWidth; x++)
                        {
                            for (int y = 0; y < terrainHeight; y++)
                            {
                                terrains[tX][tY][tZ].terrainMap[x, y, 0] = terrains[tX][tY][tZ - 1].terrainMap[x, y, terrainDepth - 1];
                            }
                        }
                    }

                    //Bottom
                    if (tY - 1 >= 0)
                    {
                        // Need to grab below chunks top row and assign it to current chunks bottom row points
                        for (int x = 0; x < terrainWidth; x++)
                        {
                            for (int z = 0; z < terrainDepth; z++)
                            {
                                terrains[tX][tY][tZ].terrainMap[x, 0, z] = terrains[tX][tY - 1][tZ].terrainMap[x, terrainHeight - 1, z];
                            }
                        }
                    }
                   
                }
            }
        }
    }

}
