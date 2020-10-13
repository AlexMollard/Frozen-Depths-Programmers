/*
    File name: Tool.cs
    Author: Michael Sweetman
    Summary: Manages the Fuel Bar display on the tool canvas
    Creation Date: 25/08/2020
    Last Modified: 13/10/2020
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
    Vector2 newSize = Vector2.zero;

    private void Start()
    {
        // get the rect transform
        bar = GetComponent<RectTransform>();
        // store the width of the bar
        newSize.x = bar.sizeDelta.x;
    }

    // Update is called once per frame
    void Update()
    {
        // determine the height of the bar using the percent of the tool capacity filled
        newSize.y = fuelBarMinHeight + (tool.toolFuel / tool.capacity * (fuelBarMaxHeight - fuelBarMinHeight));
        // set the bar to its new size
        bar.sizeDelta = newSize;
    }
}
