using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BreathValueController : MonoBehaviour
{
    TextMeshProUGUI breathValue;
    private void Start()
    {
        breathValue = gameObject.GetComponent<TextMeshProUGUI>();
        
    }
    public void updateValue(float value)
    {
        breathValue.text = Mathf.RoundToInt(value) + " Seconds";
    }

    
}
