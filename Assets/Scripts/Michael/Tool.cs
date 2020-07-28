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
        if (Input.GetMouseButton(0) && toolFuel > 0.0f ||
            Input.GetMouseButton(1) && toolFuel < 100.0f)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f,1.0f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // blowtorch
                if (Input.GetMouseButton(0))
                {
                    if (hit.transform.tag == "Ice")
                    {
                        hit.transform.GetComponent<TerrainManager>().Burn(hit.point, freezeRadius);
                        toolFuel -= Time.deltaTime * blowtorchFuelLossRate;
                    }
                }
                // freezer
                else
                {
                    if (hit.transform.tag == "Ice")
                    {
                        hit.transform.GetComponent<TerrainManager>().Freeze(hit.point, freezeRadius);
                        toolFuel += Time.deltaTime * freezeFuelLossRate;
                    }
                }
            }
        }
    }
}
