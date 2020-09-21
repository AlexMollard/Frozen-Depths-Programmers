/*
    File name: TerrainMan.cs
    Author: Alex Mollard
    Summary: Manage all editable terrain to help update them all and keep all vertices in sync.
    Creation Date: 21/07/2020
    Last Modified: 14/09/2020
*/

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

public class aabb
{
    public aabb(Vector3 pointOne, Vector3 pointTwo)
    {
        posOne = pointOne;
        posTwo = pointTwo;
    }

    public bool IsColliding(Vector3Int point)
    {
        return ((posOne.x > point.x && posOne.y > point.y && posOne.z > point.z)
            && (posTwo.x < point.x && posTwo.y < point.y && posTwo.z < point.z) );
    }

    public Vector3 posOne;
    public Vector3 posTwo;
}


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
        PreMade,
        CubePreMade
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
    public int chunkSize = 8;

    [Header("Geometry Settings")]
    [Tooltip("Changing these values will change how lighting and terrain manipulation effects a mesh")]
    public bool smoothTerrain = true;
    public bool flatShaded = true;

    [Header("Chunk Spawn Settings")]
    [Tooltip("Changing these values will change how each chunk will be populated")]
    public spawnPrefabs chunkPrefab = 0;

    public List<aabb> fillSpots = new List<aabb>();
    public List<Vector3Int> dirtyChunks = new List<Vector3Int>();

    Vector3 centerOfMeshes;
    Vector3 ChunkTotal;

    MeshRenderer[] chunkRenderers;

    void OnDrawGizmosSelected()
    {
        float halfChunkSize = chunkSize - 1;

        Vector3 currentManPos = transform.position;
        Vector3 scale = new Vector3((halfChunkSize - 1) * (terrainTotalX), (halfChunkSize - 1) * (terrainTotalY), (halfChunkSize - 1) * (terrainTotalZ));

        Vector3 centerOfMeshes = new Vector3(scale.x * 0.5f, scale.y * 0.5f, scale.z * 0.5f) + currentManPos;
        
        Gizmos.color = new Color(1, 1, 0, 0.55f);
        Gizmos.DrawCube(centerOfMeshes, scale);

        Gizmos.color = new Color(1, 0, 1, 0.65f);

        switch (chunkPrefab)
        {
            case spawnPrefabs.FlatAtBottom:
                Gizmos.DrawCube(centerOfMeshes - new Vector3(0, scale.y * 0.5f - halfChunkSize, 0), new Vector3(scale.x, halfChunkSize, scale.z));
                break;
            case spawnPrefabs.FlatAtTop:
                Gizmos.DrawCube(centerOfMeshes - new Vector3(0, halfChunkSize, 0), new Vector3(scale.x, scale.y - halfChunkSize, scale.z));
                break;
            case spawnPrefabs.HalfFill:
                Gizmos.DrawCube(centerOfMeshes - new Vector3(0, scale.y * 0.25f, 0), new Vector3(scale.x, scale.y * 0.5f, scale.z));
                break;
            case spawnPrefabs.Bowl:
                Gizmos.DrawCube(centerOfMeshes - new Vector3(0, scale.y * 0.5f - halfChunkSize, 0), new Vector3(scale.x, halfChunkSize, scale.z));
                Gizmos.DrawCube(centerOfMeshes + new Vector3(scale.x * 0.375f, 0, 0), new Vector3(scale.x / 4, scale.y, scale.z));
                Gizmos.DrawCube(centerOfMeshes - new Vector3(scale.x * 0.375f, 0, 0), new Vector3(scale.x / 4, scale.y, scale.z));
                Gizmos.DrawCube(centerOfMeshes + new Vector3(0, 0, scale.z * 0.375f), new Vector3(scale.x, scale.y, scale.z / 4));
                Gizmos.DrawCube(centerOfMeshes - new Vector3(0, 0, scale.z * 0.375f), new Vector3(scale.x, scale.y, scale.z / 4));
                break;
            case spawnPrefabs.XPlusWall:
                Gizmos.DrawCube(centerOfMeshes + new Vector3(scale.x * 0.25f, 0, 0), new Vector3(scale.x / 2, scale.y, scale.z));
                break;
            case spawnPrefabs.XNegWall:
                Gizmos.DrawCube(centerOfMeshes - new Vector3(scale.x * 0.25f, 0, 0), new Vector3(scale.x / 2, scale.y, scale.z));
                break;
            case spawnPrefabs.ZPlusWall:
                Gizmos.DrawCube(centerOfMeshes + new Vector3(0, 0, scale.z * 0.25f), new Vector3(scale.x, scale.y, scale.z / 2));
                break;
            case spawnPrefabs.ZNegWall:
                Gizmos.DrawCube(centerOfMeshes - new Vector3(0, 0, scale.z * 0.25f), new Vector3(scale.x, scale.y, scale.z / 2));
                break;
            case spawnPrefabs.PreMade:
                    Gizmos.DrawCube(centerOfMeshes, new Vector3(halfChunkSize, halfChunkSize, halfChunkSize));
                break;
            default:
                break;
        }
    }

    void Start()
    {
        foreach (Transform child in transform)
        {
            Vector3 halfScale = new Vector3(child.localScale.x / 2.0f + 1, child.localScale.y / 2.0f + 1, child.localScale.z / 2.0f + 1);
            aabb newAABB = new aabb(child.position + halfScale, child.position - halfScale);
            fillSpots.Add(newAABB);
            child.GetComponent<MeshRenderer>().enabled = false;
        }

        Vector3 currentManPos = transform.position;
        chunkRenderers = new MeshRenderer[terrainTotalX * terrainTotalY * terrainTotalZ];
        centerOfMeshes = new Vector3((chunkSize * terrainTotalX) * 0.5f, (chunkSize * terrainTotalY) * 0.5f, (chunkSize * terrainTotalZ) * 0.5f) + currentManPos;
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
                    terrainsOBJS[x][y].Add(Instantiate(terrainPrefab, new Vector3(x * (chunkSize - 1), y * (chunkSize - 1), z * (chunkSize - 1)) + currentManPos, Quaternion.identity));
                    terrainsOBJS[x][y][z].name = x + ", " + y + ", " + z;
                    terrains[x][y].Add(terrainsOBJS[x][y][z].GetComponent<EditableTerrain>());
                    terrains[x][y][z].spawnPrefab = chunkPrefab;
                    terrains[x][y][z].CreateMesh(this, new Vector3Int(x,y, z), new Vector3Int(chunkSize - 1, chunkSize - 1, chunkSize - 1));
                    terrains[x][y][z].flatShaded = flatShaded;
                    terrains[x][y][z].smoothTerrain = smoothTerrain;
                    terrains[x][y][z].transform.parent = transform;

                    chunkRenderers[i] = terrains[x][y][z].GetComponent<MeshRenderer>();
                    i++;
                }
            }
        }




        AssignEdgeValues();
        
        if (chunkPrefab == spawnPrefabs.PreMade)
            LoadMesh();
        else
            PopulateAllChunks();

        AssignEdgeValues();
        
        RefreshAllChunks();
    }

    bool isInDistance = true;
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
            RefreshAllChunks();

       // if (isInDistance == true)
       // {
       //     if (Vector3.Distance(player.transform.position, centerOfMeshes) > maxDistanceFromMesh)
       //     {
       //         isInDistance = false;
       //         for (int i = 0; i < chunkRenderers.Length; i++)
       //             chunkRenderers[i].enabled = false;
       //
       //         Debug.Log("Disabled renderers");
       //     }
       // }
       // else if(Vector3.Distance(player.transform.position, centerOfMeshes) <= maxDistanceFromMesh)
       // {
       //     if (isInDistance == false)
       //     {
       //         for (int i = 0; i < chunkRenderers.Length; i++)
       //             chunkRenderers[i].enabled = true;
       //     }
       //
       //         Debug.Log("Enabled renderers");
       //     isInDistance = true;
       // }
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
                    for (int xIN = 0; xIN < chunkSize; xIN++)
                    {
                        for (int yIN = 0; yIN < chunkSize; yIN++)
                        {
                            for (int zIN = 0; zIN < chunkSize; zIN++)
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
                    for (int xIN = 0; xIN < chunkSize; xIN++)
                    {
                        for (int yIN = 0; yIN < chunkSize; yIN++)
                        {
                            for (int zIN = 0; zIN < chunkSize; zIN++)
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
                    for (int xIN = 0; xIN < chunkSize; xIN++)
                    {
                        for (int yIN = 0; yIN < chunkSize; yIN++)
                        {
                            for (int zIN = 0; zIN < chunkSize; zIN++)
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

    public void UpdateChunk(bool isFreeze, Vector3Int publicVertHitPoint, Vector3Int chunkIndex, float beamRadius, float beamStrength)
    {
        if (chunkIndex.x >= 0 && chunkIndex.x < terrainTotalX && chunkIndex.y >= 0 && chunkIndex.y < terrainTotalY && chunkIndex.z >= 0 && chunkIndex.z < terrainTotalZ)
        {
            //if (!dirtyChunks.Contains(chunkIndex))
            //{
            //    terrains[chunkIndex.x][chunkIndex.y][chunkIndex.z].EditTerrain(isFreeze,publicVertHitPoint,beamRadius,beamStrength);
            //}

            terrains[chunkIndex.x][chunkIndex.y][chunkIndex.z].CreateMeshData();
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
                        for (int x = 0; x < chunkSize; x++)
                        {
                            for (int z = 0; z < chunkSize; z++)
                            {
                                terrains[tX][tY][tZ].terrainMap[x, chunkSize - 1, z] = terrains[tX][tY + 1][tZ].terrainMap[x, 0, z];
                            }
                        }
                    }


                    //Right
                    if (tX + 1 < terrainTotalX)
                    {
                        // Need to grab right chunks left row and assign it to current chunks right row points
                        for (int z = 0; z < chunkSize; z++)
                        {
                            for (int y = 0; y < chunkSize; y++)
                            {
                                terrains[tX][tY][tZ].terrainMap[chunkSize - 1, y, z] = terrains[tX + 1][tY][tZ].terrainMap[0, y, z];
                            }
                        }
                    }
                    
                    //Front
                    if (tZ + 1 < terrainTotalZ)
                    {
                        // Need to grab above chunks bottom row and assign it to current chunks top row points
                        for (int x = 0; x < chunkSize; x++)
                        {
                            for (int y = 0; y < chunkSize; y++)
                            {
                                terrains[tX][tY][tZ].terrainMap[x, y, chunkSize - 1] = terrains[tX][tY][tZ + 1].terrainMap[x, y, 0];
                            }
                        }
                    }


                    //Left
                    if (tX - 1 >= 0)
                    {
                        // Need to grab left chunks right row and assign it to current chunks left row points
                        for (int z = 0; z < chunkSize; z++)
                        {
                            for (int y = 0; y < chunkSize; y++)
                            {
                                terrains[tX][tY][tZ].terrainMap[0, y, z] = terrains[tX - 1][tY][tZ].terrainMap[chunkSize - 1, y, z];
                            }
                        }
                    }

                    //Back
                    if (tZ - 1 >= 0)
                    {
                        // Need to grab below chunks top row and assign it to current chunks bottom row points
                        for (int x = 0; x < chunkSize; x++)
                        {
                            for (int y = 0; y < chunkSize; y++)
                            {
                                terrains[tX][tY][tZ].terrainMap[x, y, 0] = terrains[tX][tY][tZ - 1].terrainMap[x, y, chunkSize - 1];
                            }
                        }
                    }

                    //Bottom
                    if (tY - 1 >= 0)
                    {
                        // Need to grab below chunks top row and assign it to current chunks bottom row points
                        for (int x = 0; x < chunkSize; x++)
                        {
                            for (int z = 0; z < chunkSize; z++)
                            {
                                terrains[tX][tY][tZ].terrainMap[x, 0, z] = terrains[tX][tY - 1][tZ].terrainMap[x, chunkSize - 1, z];
                            }
                        }
                    }
                   
                }
            }
        }
    }

}
