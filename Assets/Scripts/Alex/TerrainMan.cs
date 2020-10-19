/*
    File name: TerrainMan.cs
    Author: Alex Mollard
    Summary: Manage all editable terrain to help update them all and keep all vertices in sync.
    Creation Date: 21/07/2020
    Last Modified: 14/09/2020
*/

using System;
using System.Collections.Generic;
using UnityEngine;

public class aabb
{
    public aabb(Vector3 pointOne, Vector3 pointTwo, Vector3 pos, Vector3 scale)
    {
        posOne = pointOne;
        posTwo = pointTwo;
        centerPos = pos;
        this.scale = scale;
    }

    public bool checkCollision(Vector3Int point)
    {
        return ((posOne.x > point.x && posOne.y > point.y && posOne.z > point.z)
            && (posTwo.x < point.x && posTwo.y < point.y && posTwo.z < point.z));
    }

    public bool checkCollision(Vector3 secondCenterPos, Vector3 secondScale)
    {
        //check the X axis
        if (centerPos.x - secondCenterPos.x < scale.x + secondScale.x)
        {
            //check the Y axis
            if (centerPos.y - secondCenterPos.y < scale.y + secondScale.y)
            {
                //check the Z axis
                if (centerPos.z - secondCenterPos.z < scale.z + secondScale.z)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public Vector3 posOne;
    public Vector3 posTwo;

    public Vector3 centerPos;
    public Vector3 scale;
}

public class TerrainMan : MonoBehaviour
{
    // Stores chunks
    private List<List<List<EditableTerrain>>> terrains = new List<List<List<EditableTerrain>>>();

    private List<List<List<GameObject>>> terrainsOBJS = new List<List<List<GameObject>>>();

    [Header("Prefabs")]
    [Tooltip("Prefabs required to use terrain manager")]
    [SerializeField] private GameObject terrainPrefab;

    // This will most likely be changed as it is not optimal and can be very messy
    [Header("Optimizations")]
    [Tooltip("Variables that will greatly effect FPS")]
    public float maxDistanceFromMesh = 100;

    [Header("Total Chunks")]
    [Tooltip("This will determine how many chunks will be spawned in each axis")]
    [Range(1, 24)] public int terrainTotalX = 10;

    [Range(1, 24)] public int terrainTotalY = 10;
    [Range(1, 24)] public int terrainTotalZ = 10;

    [Header("Single Chunk Size")]
    [Tooltip("Terrain Width: X, TerrainHeight: Y, TerrainDepth: Z")]
    private int chunkSize = 8;

    [Header("Chunk Spawn Settings")]
    [Tooltip("Changing these values will change how each chunk will be populated")]
    public List<aabb> fillSpots = new List<aabb>();

    //Chunk varialbles
    private MeshRenderer[] chunkRenderers;

    [NonSerialized] public Vector3 centerOfMeshes;

    // Misc variables
    private int chunkRendererCount = 0;

    private Vector3 currentManPos;
    private int fillSpotsMatched = 0;

    [NonSerialized]
    public bool readFromFile = false, updateReadFile = false, missingAABB = false, missingCache = false, changedAABB = false;

    // Draw the yellow gizmo to know how large the managers are.
    private void OnDrawGizmosSelected()
    {
        float halfChunkSize = chunkSize - 1;

        Vector3 currentManPos = transform.position;
        Vector3 scale = new Vector3((halfChunkSize) * (terrainTotalX), (halfChunkSize) * (terrainTotalY), (halfChunkSize) * (terrainTotalZ));
        Vector3 centerOfMeshes = new Vector3(scale.x * 0.5f, scale.y * 0.5f, scale.z * 0.5f) + currentManPos;

        Gizmos.color = new Color(1, 1, 0, 0.55f);
        Gizmos.DrawCube(centerOfMeshes, scale);
    }

    private void Awake()
    {
        currentManPos = transform.position;
        chunkRenderers = new MeshRenderer[terrainTotalX * terrainTotalY * terrainTotalZ];
        centerOfMeshes = new Vector3((chunkSize * terrainTotalX) * 0.5f, (chunkSize * terrainTotalY) * 0.5f, (chunkSize * terrainTotalZ) * 0.5f) + currentManPos;

        // Add all cubes to aabb array for later comparisons
        foreach (Transform child in transform)
        {
            Vector3 halfScale = new Vector3(child.localScale.x / 2.0f + 1, child.localScale.y / 2.0f + 1, child.localScale.z / 2.0f + 1);
            aabb newAABB = new aabb(child.position + halfScale, child.position - halfScale, child.position, child.localScale);
            fillSpots.Add(newAABB);
            child.gameObject.SetActive(false);
        }

        if (CheckIfCacheExists())
        {
            if (fillSpots.Count > 0)
            {
                if (CheckIfAABBCacheExists())
                {
                    LoadMesh(false);

                    if (fillSpots.Count * 6 == fillSpotsMatched - 3)
                        readFromFile = true;
                    else
                    {
                        changedAABB = true;
                        SaveMesh(false);
                        updateReadFile = true;
                    }
                }
                else
                {
                    SaveMesh(false);
                    updateReadFile = true;
                }
            }
        }
        else
        {
            SaveMesh(false);
            updateReadFile = true;
        }
    }

    private bool CheckIfCacheExists()
    {
        if (System.IO.File.Exists(Application.dataPath + "\\TerrainSaves\\" + gameObject.name + ".dat"))
        {
            missingCache = false;
            return true;
        }

        missingCache = true;
        return false;
    }

    private bool CheckIfAABBCacheExists()
    {
        if (System.IO.File.Exists(Application.dataPath + "\\TerrainSaves\\" + gameObject.name + "AABB" + ".dat"))
        {
            missingAABB = false;
            return true;
        }

        missingAABB = true;
        return false;
    }

    private void CreateManager()
    {
        if (updateReadFile)
            SaveMesh(true);

        if (readFromFile)
            LoadMesh(true);

        AssignEdgeValues();
        RefreshAllChunks();
    }

    public bool StartCreation()
    {
        GenerateSomeChunks();
        if (updateReadFile)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GenerateSomeChunks()
    {
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
                    terrains[x][y][z].CreateMesh(this, new Vector3Int(x, y, z), new Vector3Int(chunkSize - 1, chunkSize - 1, chunkSize - 1));
                    terrains[x][y][z].transform.parent = transform;

                    if (!readFromFile)
                        terrains[x][y][z].PopulateTerrainMap();

                    chunkRenderers[chunkRendererCount] = terrains[x][y][z].GetComponent<MeshRenderer>();
                    chunkRendererCount++;
                }
            }
        }

        CreateManager();
    }

    public void SaveMesh(bool meshData)
    {
        System.IO.FileStream oFileStream = null;
        if (meshData)
        {
            oFileStream = new System.IO.FileStream(Application.dataPath + "\\TerrainSaves\\" + gameObject.name + ".dat", System.IO.FileMode.Create);
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
        }
        else
        {
            oFileStream = new System.IO.FileStream(Application.dataPath + "\\TerrainSaves\\" + gameObject.name + "AABB" + ".dat", System.IO.FileMode.Create);

            oFileStream.Write(BitConverter.GetBytes(transform.position.x), 0, BitConverter.GetBytes(transform.position.x).Length);
            oFileStream.Write(BitConverter.GetBytes(transform.position.y), 0, BitConverter.GetBytes(transform.position.y).Length);
            oFileStream.Write(BitConverter.GetBytes(transform.position.z), 0, BitConverter.GetBytes(transform.position.z).Length);

            foreach (aabb cube in fillSpots)
            {
                // Center Pos
                oFileStream.Write(BitConverter.GetBytes(cube.centerPos.x), 0, BitConverter.GetBytes(cube.centerPos.x).Length);
                oFileStream.Write(BitConverter.GetBytes(cube.centerPos.y), 0, BitConverter.GetBytes(cube.centerPos.y).Length);
                oFileStream.Write(BitConverter.GetBytes(cube.centerPos.z), 0, BitConverter.GetBytes(cube.centerPos.z).Length);

                // Scale
                oFileStream.Write(BitConverter.GetBytes(cube.scale.x), 0, BitConverter.GetBytes(cube.scale.x).Length);
                oFileStream.Write(BitConverter.GetBytes(cube.scale.y), 0, BitConverter.GetBytes(cube.scale.y).Length);
                oFileStream.Write(BitConverter.GetBytes(cube.scale.z), 0, BitConverter.GetBytes(cube.scale.z).Length);
            }
        }

        oFileStream.Close();
    }

    public bool LoadMesh(bool meshData)
    {
        if (meshData)
        {
            if (!CheckIfCacheExists())
            {
                PopulateAllChunks();
                return false;
            }
            System.IO.FileStream oFileStream = null;
            oFileStream = new System.IO.FileStream(Application.dataPath + "\\TerrainSaves\\" + gameObject.name + ".dat", System.IO.FileMode.Open);

            int length = (int)oFileStream.Length;  // get file length
            byte[] buffer = new byte[length];      // create buffer
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
                                    terrains[x][y][z].terrainMap[xIN, yIN, zIN] = new FloatMyGuy(0.0f);
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
        }
        else
        {
            System.IO.FileStream oFileStream = null;
            oFileStream = new System.IO.FileStream(Application.dataPath + "\\TerrainSaves\\" + gameObject.name + "AABB" + ".dat", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);

            int length = (int)oFileStream.Length;  // get file length
            byte[] buffer = new byte[length];      // create buffer
            int count;                            // actual number of bytes read
            int sum = 0;                          // total number of bytes read

            foreach (aabb cube in fillSpots)
            {
                for (int i = 0; i < 6; i++)
                {
                    count = oFileStream.Read(buffer, sum, length - sum);
                    sum += count;
                }
            }

            int offset = 0;

            //Manager Pos
            if (BitConverter.ToSingle(buffer, offset) == transform.position.x)
                fillSpotsMatched++;
            offset += 4;

            if (BitConverter.ToSingle(buffer, offset) == transform.position.y)
                fillSpotsMatched++;
            offset += 4;

            if (BitConverter.ToSingle(buffer, offset) == transform.position.z)
                fillSpotsMatched++;
            offset += 4;

            foreach (aabb cube in fillSpots)
            {
                if (BitConverter.ToSingle(buffer, offset) == cube.centerPos.x)
                    fillSpotsMatched++;
                offset += 4;

                if (BitConverter.ToSingle(buffer, offset) == cube.centerPos.y)
                    fillSpotsMatched++;
                offset += 4;

                if (BitConverter.ToSingle(buffer, offset) == cube.centerPos.z)
                    fillSpotsMatched++;
                offset += 4;

                if (BitConverter.ToSingle(buffer, offset) == cube.scale.x)
                    fillSpotsMatched++;
                offset += 4;

                if (BitConverter.ToSingle(buffer, offset) == cube.scale.y)
                    fillSpotsMatched++;
                offset += 4;

                if (BitConverter.ToSingle(buffer, offset) == cube.scale.z)
                    fillSpotsMatched++;
                offset += 4;
            }
            oFileStream.Close();
        }

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

    private void AssignEdgeValues()
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