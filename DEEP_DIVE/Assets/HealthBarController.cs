using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarController : MonoBehaviour
{
    public Image healthBar;
    public CharacterController character;
    private bool isSubmerged = false;

    // Update is called once per frame
    void UpdateHealthBar()
    {
        healthBar.fillAmount = healthBar.fillAmount - .1f;
        
    }
    float elapsed = 0f;
    void Update()
    {
        if (character.transform.position.y < 113) {
            Debug.Log("Sumberged");
            isSubmerged = true;
        }
        if (isSubmerged) 
        {
            elapsed += Time.deltaTime;
            if (elapsed >= 1f)
            {
                elapsed = elapsed % 1f;
                UpdateHealthBar();
            }
        }
        

    }

}
