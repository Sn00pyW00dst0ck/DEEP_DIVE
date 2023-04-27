using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnableOnPress : MonoBehaviour
{
    public InputActionReference menuActivatorReference = null;
    public GameObject myMenu;
    private bool toggle;

    // Start is called before the first frame update
    void Start()
    {
        toggle = true;
    }

    private void Awake()
    {
        menuActivatorReference.action.started += ToggleMenu;
    }

    private void OnDestroy()
    {
        menuActivatorReference.action.started -= ToggleMenu;
    }

    private void ToggleMenu(InputAction.CallbackContext context)
    {
        if (toggle == false)
        {
            toggle = true;
            myMenu.active = true;

        }
        else
        {
            toggle = false;
            myMenu.active = false;
        }
    }
}
