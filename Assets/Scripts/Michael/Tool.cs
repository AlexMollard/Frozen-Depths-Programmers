using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    [Header("Blowtorch")]
    public float burnRadius;
    public float blowtorchFuelLossRate;

    [Header("Freezer")]
    public float freezeRadius;
    public float freezeFuelLossRate;

    float toolFuel = 50.0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && toolFuel > 0.0f ||
            Input.GetMouseButtonDown(1) && toolFuel < 100.0f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit) && hit.collider.gameObject.CompareTag("Terrain"))
            {
                TerrainManipulation terrain = hit.collider.gameObject.GetComponent<TerrainManipulation>();

                // blowtorch
                if (Input.GetMouseButtonDown(0))
                {
                    //terrain.Burn(hit.point, burnRadius);
                    toolFuel -= Time.deltaTime * blowtorchFuelLossRate;
                }
                // freezer
                else
                {
                    //terrain.Freeze(hit.point, freezeRadius);
                    toolFuel += Time.deltaTime * freezeFuelLossRate;
                }
            }
        }
    }
}
