using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tool : MonoBehaviour
{
    [Header("Tool Use")]
    public float range = 10.0f;
    public float radius;

    [Header("Fuel Economy")]
    [HideInInspector]
    public float toolFuel = 0.0f;
    public float FuelGainRate = 100.0f;
    public float FuelLossRate = 100.0f;
    public float capacity = 1000.0f;

    [Header("FuelDisplay")]
    public Text fuelDisplay;

    [Header("Other")]
    /*public MenuManager menu;*/
    public Camera playerCamera;

    void Update()
    {
        if (/*menu.inGame &&*/
            Input.GetMouseButton(0)||
            Input.GetMouseButton(1) && toolFuel > 0.0f)
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f,0.5f,1.0f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, range))
            {
                // blowtorch
                if (Input.GetMouseButton(0))
                {
                    if (hit.transform.tag == "Ice")
                    {
                        if (hit.transform.GetComponent<EditableTerrain>().Burn(hit.point, radius))
                        {
                            toolFuel += Time.deltaTime * FuelGainRate;
                            if (capacity > 0.0f && toolFuel > capacity)
                            {
                                toolFuel = capacity;
                            }
                        }
                    }
                }
                // freezer
                else
                {
                    if (hit.transform.tag == "Ice")
                    {
                        if (hit.transform.GetComponent<EditableTerrain>().Freeze(hit.point, radius))
                        {
                            toolFuel -= Time.deltaTime * FuelLossRate;
                            if (toolFuel < 0.0f)
                            {
                                toolFuel = 0.0f;
                            }
                        }
                    }
                }
            }
            fuelDisplay.text = toolFuel.ToString("F2") + "mL";
        }
    }
}
