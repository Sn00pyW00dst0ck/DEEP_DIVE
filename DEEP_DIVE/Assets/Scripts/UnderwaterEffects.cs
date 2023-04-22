using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterEffects : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject[] underwaterEffects;

    [SerializeField]
    ToggleLocomotion toggler;

    [SerializeField]
    Terrain[] underwaterTerrains;

    [SerializeField]
    Terrain[] aboveGroundTerrains;

    [SerializeField]
    GameObject[] underwaterAssets;

    [SerializeField]
    GameObject[] aboveGroundAssets;


    [SerializeField]
    Material skyboxWithoutFog;

    [SerializeField]
    Material skyboxWithFog;

    public GameObject healthBar;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("MainCamera"))
        {
            RenderSettings.fog = true;
            RenderSettings.skybox = skyboxWithFog;

            foreach (GameObject obj in underwaterEffects)
            {
                obj.SetActive(true);
            }

            toggler.EnterWater();

            // Turn on all the underwater trees
            foreach (Terrain obj in underwaterTerrains)
            {
                obj.drawTreesAndFoliage = true;
            }

            foreach (Terrain obj in aboveGroundTerrains)
            {
                obj.drawTreesAndFoliage = false;
            }

            healthBar.SetActive(true);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("MainCamera"))
        {
            RenderSettings.fog = false;
            RenderSettings.skybox = skyboxWithoutFog;


            //foreach (GameObject obj in underwaterEffects)
            //{
            //    obj.SetActive(false);
            //}

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }

            toggler.EnterLand();

            // Turn off all the underwater trees
            foreach (Terrain obj in underwaterTerrains)
            {
                obj.drawTreesAndFoliage = false;
            }

            foreach (Terrain obj in aboveGroundTerrains)
            {
                obj.drawTreesAndFoliage = true;
            }

            healthBar.SetActive(false);
        }
        
    }
}
