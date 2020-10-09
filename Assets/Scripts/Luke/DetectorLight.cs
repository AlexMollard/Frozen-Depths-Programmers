using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorLight : MonoBehaviour
{
    [SerializeField]
    private MetalDetector detector;
    [SerializeField]
    private Color lightUpColour;
    [SerializeField]
    private float maxDistance;
    [SerializeField]
    private float minDistance;

    private Color lightOffColour = new Color(1, 1, 1);

    [SerializeField]
    private bool debugLogs = false;

    private void Update()
    {
        float dis = detector.GetDistance();
        Material mat = GetComponent<MeshRenderer>().material;

        if (dis > minDistance && dis < maxDistance)
        {
            mat.color = lightUpColour;
            mat.SetColor("_EmissionColor", lightUpColour);
        }
        else
        {
            mat.color = lightOffColour;
            mat.SetColor("_EmissionColor", lightOffColour);
        }

        if (debugLogs) Debug.Log("Detector Light: Distance from closest entrance is " + dis);
    }
}
