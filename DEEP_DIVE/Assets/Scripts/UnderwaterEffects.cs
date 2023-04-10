using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterEffects : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject underwaterEffects;

    [SerializeField]
    Terrain[] underwaterTerrains;

    private void OnTriggerEnter(Collider other)
    {
        RenderSettings.fog = true;
        underwaterEffects.SetActive(true);
        // Turn on all the underwater trees
        foreach (Terrain obj in underwaterTerrains)
        {
            obj.drawTreesAndFoliage = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RenderSettings.fog = false;
        underwaterEffects.SetActive(false);
        // Turn off all the underwater trees
        foreach (Terrain obj in underwaterTerrains)
        {
            obj.drawTreesAndFoliage = false;
        }
    }
}
