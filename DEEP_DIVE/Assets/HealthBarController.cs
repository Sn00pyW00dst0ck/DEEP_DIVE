using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarController : MonoBehaviour
{
    public Image healthBar;
    public float maxTime;
    private float timeIncrement;
    private float startTime = 0f;


    private void OnEnable()
    {
        healthBar.fillAmount = 1;
        timeIncrement = maxTime / 10;
        startTime = Time.time;
    }

    void Update()
    {
        
        
        
        if (Time.time - startTime >= timeIncrement)
        {
            startTime = Time.time;
            healthBar.fillAmount = healthBar.fillAmount - .1f;
        }
        
        

    }

}
