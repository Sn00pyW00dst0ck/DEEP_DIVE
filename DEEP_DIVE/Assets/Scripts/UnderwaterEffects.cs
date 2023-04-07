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
    GameObject[] underwaterTerrains;

    private void OnTriggerEnter(Collider other)
    {
        RenderSettings.fog = true;
        underwaterEffects.SetActive(true);
        foreach (GameObject obj in underwaterTerrains)
        {
            obj.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RenderSettings.fog = false;
        underwaterEffects.SetActive(false);
        foreach (GameObject obj in underwaterTerrains)
        {
            obj.SetActive(false);
        }
    }
}
