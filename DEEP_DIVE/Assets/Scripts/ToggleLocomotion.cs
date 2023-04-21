using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ToggleLocomotion : MonoBehaviour
{
    public ContinuousMoveProviderBase walker;
    public ContinuousMoveProviderBase swimmer;
    public Rigidbody myRigid;

    private bool swim = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SwitchLocomotion()
    {
        if(swim == false)
        {
            swim = true;
            myRigid.useGravity = true;
            walker.enabled = false;
            swimmer.enabled = true;
        }
        else
        {
            swim = false;
            myRigid.useGravity = false;
            walker.enabled = true;
            swimmer.enabled = false;
        }
    }

    public void EnterWater()
    {
        Debug.Log("switched");
        swim = true;
        myRigid.useGravity = false;
        walker.enabled = false;
        swimmer.enabled = true;
    }

    public void EnterLand()
    {
        swim = false;
        myRigid.useGravity = true;
        walker.enabled = true;
        swimmer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
