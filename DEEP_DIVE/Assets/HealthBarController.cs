using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarController : MonoBehaviour
{
    public Image healthBar;


    
    private void OnEnable()
    {
        healthBar.fillAmount = 1;
    }

    float elapsed = 0f;
    void Update()
    {
        
        
        
        elapsed += Time.deltaTime;
        if (elapsed >= 1f)
        {
            elapsed = elapsed % 1f;
            healthBar.fillAmount = healthBar.fillAmount - .1f;
        }
        
        

    }

}
