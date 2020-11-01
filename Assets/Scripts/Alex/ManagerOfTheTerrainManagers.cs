/*
    File name: ManagerOfTheTerrainManagers.cs
    Author: Alex Mollard
    Summary: Used to load all managers at the start of a level.
    Creation Date: 12/10/2020
    Last Modified: 12/10/2020
*/

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerOfTheTerrainManagers : MonoBehaviour
{
    public GameObject player = null;

    [Header("Managers")]
    [Tooltip("All terrain managers")]
    public TerrainMan[] terrainMans = null;

    [Header("Ui Variables")]
    [Tooltip("Visual input on how many managers are loaded and or being cached")]
    public GameObject loadingScreen = null;

    public Slider loadingBar = null;
    public TextMeshProUGUI cachingText = null;
    public TextMeshProUGUI missingAABBCacheText = null;
    public TextMeshProUGUI missingMeshCacheText = null;
    public TextMeshProUGUI changedAABBCacheText = null;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(DoLoad());
    }

    public TerrainMan[] GetManagers(int ID)
    {
        int size = 0;
        for (int i = 0; i < terrainMans.Length; i++)
        {
            if (terrainMans[i].CheckPointID == ID)
                size++;
        }

        if (size == 0)
            return null;

        int count = 0;
        TerrainMan[] tempTerrains = new TerrainMan[size];
        for (int i = 0; i < terrainMans.Length; i++)
        {
            if (terrainMans[i].CheckPointID != ID)
                continue;

            tempTerrains[count] = tempTerrains[i];
            count++;
        }

        return tempTerrains;

    }

    private IEnumerator DoLoad()
    {
        for (int i = 0; i < terrainMans.Length; i++)
        {
            if (terrainMans[i].StartCreation())
            {
                if (cachingText)
                    cachingText.text = "Caching current terrain (" + i + "/" + terrainMans.Length + ")...";
            }
            else
            {
                if (cachingText)
                    cachingText.text = "Reading From Cache (" + i + "/" + terrainMans.Length + ")...";
            }
            if (missingAABBCacheText)
                missingAABBCacheText.gameObject.SetActive(terrainMans[i].missingAABB);

            if (missingMeshCacheText)
                missingMeshCacheText.gameObject.SetActive(terrainMans[i].missingCache);

            if (changedAABBCacheText)
                changedAABBCacheText.gameObject.SetActive(terrainMans[i].changedAABB);

            if (loadingBar)
                loadingBar.value = (float)(i + 1) * (1.0f / (float)terrainMans.Length);
            yield return 0;
        }

        if (loadingScreen)
            loadingScreen.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Disable mesh when far away from player
        for (int i = 0; i < terrainMans.Length; i++)
        {
            if (Vector3.Distance(player.transform.position, terrainMans[i].centerOfMeshes) > terrainMans[i].maxDistanceFromMesh)
            {
                if (terrainMans[i].gameObject.activeSelf)
                    terrainMans[i].gameObject.SetActive(false);
            }
            else
            {
                if (!terrainMans[i].gameObject.activeSelf)
                    terrainMans[i].gameObject.SetActive(true);
            }
        }
    }
}