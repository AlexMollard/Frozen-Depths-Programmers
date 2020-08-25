/*
    File name: Tool.cs
    Author: Michael Sweetman
    Summary: Manages the Fuel Bar display on the tool canvas
    Creation Date: 25/08/2020
    Last Modified: 25/08/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelBar : MonoBehaviour
{
    public Tool tool;
    public float fuelBarMaxWidth = 300.0f;
    public float fuelBarMinWidth = 0.0f;

    RectTransform bar;

    private void Start()
    {
        bar = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        bar.sizeDelta = new Vector2(fuelBarMinWidth + (tool.toolFuel / tool.capacity * (fuelBarMaxWidth - fuelBarMinWidth)), bar.sizeDelta.y);
    }
}
