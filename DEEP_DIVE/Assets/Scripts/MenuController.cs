using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    void toggleScript()
    { 
        
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.RightControl))
        {
            player_controller script =  GetComponent<player_controller>();
            script.enabled = false;
           
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            
        }
        if (Input.GetKey(KeyCode.RightShift))
        {
            player_controller script = GetComponent<player_controller>();
            script.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
