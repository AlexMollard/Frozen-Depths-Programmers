using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelBar : MonoBehaviour
{
    public RectTransform redBar;
    public Tool tool;
    float maxWidth;

    private void Start()
    {
        maxWidth = redBar.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        redBar.sizeDelta = new Vector2((tool.toolFuel / 100.0f) * maxWidth, redBar.rect.height);
    }
}
