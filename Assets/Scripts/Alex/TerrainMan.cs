using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TerrainMan : MonoBehaviour
{
    [SerializeField]
    GameObject terrainPrefab;

    List<List<EditableTerrain>> terrains = new List<List<EditableTerrain>>();
    List<List<GameObject>> terrainsOBJS = new List<List<GameObject>>();

    [SerializeField]
    int terrainTotalX = 4;

    [SerializeField]
    int terrainTotalZ = 4;


    [SerializeField]
    int terrainWidth = 4;

    [SerializeField]
    int terrainHeight = 4;

    [SerializeField]
    int terrainDepth = 4;

    // A value of 1 is one unit in unity space
    [SerializeField]
    float verticeDistance = 1.0f;

    public bool smoothTerrain;
    public bool flatShaded;



    void Start()
    {
        Vector3 currentManPos = transform.position;

        for (int x = 0; x < terrainTotalX; x++)
        {
            terrains.Add(new List<EditableTerrain>());
            terrainsOBJS.Add(new List<GameObject>());
            for (int z = 0; z < terrainTotalZ; z++)
            {
                terrainsOBJS[x].Add(Instantiate(terrainPrefab, new Vector3(x * (terrainWidth - 1) * verticeDistance, 0, z * (terrainDepth - 1) * verticeDistance) + currentManPos, Quaternion.identity));
                terrainsOBJS[x][z].name = x + ", " + z;
                terrains[x].Add(terrainsOBJS[x][z].GetComponent<EditableTerrain>());
                terrains[x][z].CreateMesh(this, new Vector2Int(x, z), new Vector3Int(terrainWidth - 1, terrainHeight, terrainDepth - 1), verticeDistance);
                terrains[x][z].flatShaded = flatShaded;
                terrains[x][z].smoothTerrain = smoothTerrain;
            }
        }

        AssignEdgeValues();
        RefreshAllChunks();

    }

    public void RefreshAllChunks()
    {
        for (int x = 0; x < terrainTotalX; x++)
        {
            for (int z = 0; z < terrainTotalZ; z++)
            {
                terrains[x][z].CreateMeshData();
            }
        }
    }

    public void UpdateChunk(Vector2Int index)
    {
        if (index.x >= 0 && index.x < terrainTotalX && index.y >= 0 && index.y < terrainTotalZ)
        {
            terrains[index.x][index.y].CreateMeshData();
        }
    }

   void AssignEdgeValues()
    {
        for (int tX = 0; tX < terrainTotalX; tX++)
        {
            for (int tZ = 0; tZ < terrainTotalZ; tZ++)
            {
                // The corner points will be overlaping but i think this will still be fine as this should ensure that all corners are correct for neighbouring chunks
                //tX = ->
                //tZ = /\

                //Top
                if (tZ + 1 < terrainTotalZ)
                {
                    // Need to grab above chunks bottom row and assign it to current chunks top row points
                    for (int x = 0; x < terrainWidth; x++)
                    {
                        for (int y = 0; y < terrainHeight; y++)
                        {
                            terrains[tX][tZ].terrainMap[x, y, terrainDepth - 1] = terrains[tX][tZ + 1].terrainMap[x, y, 0];
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
                            terrains[tX][tZ].terrainMap[0, y, z] = terrains[tX - 1][tZ].terrainMap[terrainWidth - 1, y, z];
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
                            terrains[tX][tZ].terrainMap[terrainWidth - 1, y, z] = terrains[tX + 1][tZ].terrainMap[0, y, z];
                        }
                    }
                }

                //Bottom
                if (tZ - 1 >= 0)
                {
                    // Need to grab below chunks top row and assign it to current chunks bottom row points
                    for (int x = 0; x < terrainWidth; x++)
                    {
                        for (int y = 0; y < terrainHeight; y++)
                        {
                            terrains[tX][tZ].terrainMap[x, y, 0] = terrains[tX][tZ - 1].terrainMap[x, y, terrainDepth - 1];
                        }
                    }
                }
            }
        }
    }

}
