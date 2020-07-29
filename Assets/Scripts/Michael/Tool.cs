using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    [Header("General")]
    public float range = 10.0f;
    public MenuManager menu;
    [HideInInspector]
    public float toolFuel = 50.0f;

    [Header("Blowtorch")]
    public float burnRadius;
    public float blowtorchFuelLossRate;

    [Header("Freezer")]
    public float freezeRadius;
    public float freezeFuelLossRate;



    void Update()
    {
        if (menu.inGame &&
            Input.GetMouseButton(0) && toolFuel > 0.0f ||
            Input.GetMouseButton(1) && toolFuel < 100.0f)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f,1.0f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, range))
            {
                // blowtorch
                if (Input.GetMouseButton(0))
                {
                    if (hit.transform.tag == "Ice")
                    {
                        hit.transform.GetComponent<TerrainManipulation>().Burn(hit.point, freezeRadius);
                        toolFuel -= Time.deltaTime * blowtorchFuelLossRate;
                    }
                }
                // freezer
                else
                {
                    if (hit.transform.tag == "Ice")
                    {
                        hit.transform.GetComponent<TerrainManipulation>().Freeze(hit.point, freezeRadius);
                        toolFuel += Time.deltaTime * freezeFuelLossRate;
                    }
                }
            }
        }
    }
}
