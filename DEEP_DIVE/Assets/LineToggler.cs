using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class LineToggler : MonoBehaviour
{
    public XRInteractorLineVisual lineVisual;
    public XRRayInteractor rayInteractor;
    public InputActionReference lineActivatorReference = null;

    private bool toggle;
    // Start is called before the first frame update
    private void Start()
    {
        toggle = true;
    }
    void Awake()
    {
        lineActivatorReference.action.started += ToggleLine;
    }

    private void OnDestroy()
    {
        lineActivatorReference.action.started -= ToggleLine;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ToggleLine(InputAction.CallbackContext context)
    {
        if(toggle == false)
        {
            toggle = true;
            rayInteractor.maxRaycastDistance = 30;
            lineVisual.enabled = true;

        }
        else
        {
            toggle = false;
            rayInteractor.maxRaycastDistance = 3;
            lineVisual.enabled = false;
        }
    }
}
