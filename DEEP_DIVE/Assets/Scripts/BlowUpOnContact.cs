using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowUpOnContact : MonoBehaviour
{
    [SerializeField]
    private GameObject destroyer;
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
       
       if (GameObject.ReferenceEquals(other.gameObject, destroyer))
       { 
            Debug.Log("first and second are the same");
            Object.Destroy(this.gameObject);
       }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
