using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCopy : MonoBehaviour
{
    [SerializeField]
    private Transform self;

    [SerializeField]
    private Transform incoming;

    [SerializeField]
    private float turnSmoothness;

    [SerializeField]
    Vector3 headBodyOffset;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        self.position = incoming.position + headBodyOffset;
        self.forward = Vector3.Slerp(self.forward, Vector3.ProjectOnPlane(incoming.forward, Vector3.up).normalized, Time.deltaTime * turnSmoothness);
    }
}
