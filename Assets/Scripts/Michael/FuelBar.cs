/*
    File name: Tool.cs
    Author: Michael Sweetman
    Summary: Manages the Fuel Bar display on the tool canvas
    Creation Date: 25/08/2020
    Last Modified: 12/10/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelBar : MonoBehaviour
{
    [SerializeField] Tool tool;
    [SerializeField] float fuelBarMaxHeight = 300.0f;
    [SerializeField] float fuelBarMinHeight = 0.0f; 

    RectTransform bar;

    private void Start()
    {
        // get the rect transform
        bar = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        // determine the height of the bar using the percent of the tool capacity filled
        bar.sizeDelta = new Vector2(bar.sizeDelta.x, fuelBarMinHeight + (tool.toolFuel / tool.capacity * (fuelBarMaxHeight - fuelBarMinHeight)));
    }
}
