using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManipulation : MonoBehaviour
{
    public GameObject testCube;

    const int width = 20;
    const int height = 20;
    public float zoom = 10.0f;
    Vector3[,,] vertices = new Vector3[width, height, width];

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    vertices[x, y, z] = new Vector3(x,y,z);

                    if (PerlinNoise3D(x / zoom, y / zoom, z / zoom) > 0.25f)
                        Instantiate(testCube, vertices[x, y, z], Quaternion.identity);
                }
            }
        }



    }

    public static float PerlinNoise3D(float x, float y, float z)
    {
        y += 1;
        z += 2;
        float xy = _perlin3DFixed(x, y);
        float xz = _perlin3DFixed(x, z);
        float yz = _perlin3DFixed(y, z);
        float yx = _perlin3DFixed(y, x);
        float zx = _perlin3DFixed(z, x);
        float zy = _perlin3DFixed(z, y);
        return xy * xz * yz * yx * zx * zy;
    }
    static float _perlin3DFixed(float a, float b)
    {
        return Mathf.Sin(Mathf.PI * Mathf.PerlinNoise(a, b));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
