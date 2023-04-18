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
    ToggleLocomotion toggler;

    [SerializeField]
    Terrain[] underwaterTerrains;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "MainCamera")
        {
            RenderSettings.fog = true;
            underwaterEffects.SetActive(true);
            toggler.EnterWater();
            // Turn on all the underwater trees
            foreach (Terrain obj in underwaterTerrains)
            {
                obj.drawTreesAndFoliage = true;
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "MainCamera")
        {
            RenderSettings.fog = false;
            underwaterEffects.SetActive(false);
            toggler.EnterLand();
            // Turn off all the underwater trees
            foreach (Terrain obj in underwaterTerrains)
            {
                obj.drawTreesAndFoliage = false;
            }
        }
        
    }
}
