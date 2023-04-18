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

    [SerializeField]
    GameObject underwaterAssets;

    [SerializeField]
    Terrain[] aboveWaterTerrains;

    [SerializeField]
    GameObject aboveWaterAssets;

    [SerializeField]
    GameObject healthBar;

    private void OnTriggerEnter(Collider other)
    {
        RenderSettings.fog = true;
        underwaterEffects.SetActive(true);
        // Turn on all the underwater trees
        foreach (Terrain obj in underwaterTerrains)
        {
            obj.drawTreesAndFoliage = true;
        }
        // Turn off all the above ground terrains
        foreach (Terrain obj in aboveWaterTerrains)
        {
            obj.drawTreesAndFoliage = false;
        }
        healthBar.SetActive(true);
        underwaterAssets.SetActive(true);
        aboveWaterAssets.SetActive(false);
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
        // Turn on all the above ground terrains
        foreach (Terrain obj in aboveWaterTerrains)
        {
            obj.drawTreesAndFoliage = true;
        }
        healthBar.SetActive(false);
        underwaterAssets.SetActive(false);
        aboveWaterAssets.SetActive(true);
    }
}
